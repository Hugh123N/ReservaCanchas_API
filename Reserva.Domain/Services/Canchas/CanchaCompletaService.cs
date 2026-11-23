using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reserva.Domain.Commands.Dbo.Cancha;
using Reserva.Domain.Queries.Dbo.Cancha;
using Reserva.Domain.Services.GestionAdmin;
using Reserva.Domain.Services.Storage;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Services.Canchas;

public class CanchaCompletaService(
    CreateCanchaCommandValidator validator, 
    UpdateCanchaCommandValidator updateValidator, 
    NegocioAccessService access,
    IPlanLimitValidationService limites, 
    IStorageService storage, 
    IRepository<ImagenCancha> imagenes,
    IRepository<Entity.Cancha> canchas, 
    IUnitOfWork unitOfWork, 
    IMediator mediator, 
    ILogger<CanchaCompletaService> logger)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { MaxDepth = 32 };

    public async Task<ResponseDto<GetCanchaDto>> CrearAsync(string datos, IFormFileCollection archivos, CancellationToken ct)
    {
        var response = new ResponseDto<GetCanchaDto>();
        CrearCanchaCompletaDto? solicitud;
        try { solicitud = datos.Length <= 131072 ? JsonSerializer.Deserialize<CrearCanchaCompletaDto>(datos, Json) : null; }
        catch (JsonException) { solicitud = null; }
        if (solicitud?.Cancha == null || archivos.Count is < 1 or > 4 || solicitud.IndicePrincipal < 0 || solicitud.IndicePrincipal >= archivos.Count)
        { response.AddErrorResult("Envía los datos de la cancha, entre una y cuatro imágenes y un índice de imagen principal válido."); return response; }

        var validacion = await validator.ValidateAsync(new CreateCanchaCommand(solicitud.Cancha), ct);
        if (!validacion.IsValid) { response.AddErrorResult(validacion); return response; }
        var proveedor = await access.ProveedorActualAsync();
        if (!access.EsProveedor || proveedor == null || proveedor.IdProveedor != solicitud.Cancha.IdProveedor || !NegocioReglas.Habilitado(proveedor))
        { response.AddErrorResult("No tienes permiso para crear canchas en este negocio."); return response; }
        if (!await limites.CanCreateCanchaAsync(proveedor.IdProveedor))
        { response.AddErrorResult("Has alcanzado el límite de canchas de tu plan."); return response; }
        if (!await SonImagenesValidasAsync(archivos, ct))
        { response.AddErrorResult("Las imágenes deben ser JPEG, PNG o WebP, no vacías y de hasta 5 MB cada una."); return response; }

        var claves = new List<string>();
        try
        {
            var urls = await SubirImagenesAsync(proveedor.IdProveedor, archivos, claves, ct);
            ct.ThrowIfCancellationRequested();
            response = await unitOfWork.ExecuteInTransactionAsync(solicitud, async (entrada, token) =>
            {
                var guardada = await mediator.Send(new CreateCanchaCommand(entrada.Cancha), token);
                if (guardada.IsValid && guardada.Data == null) guardada.AddErrorResult("No se pudo recuperar la cancha creada.");
                if (!guardada.IsValid || guardada.Data == null) throw new ResultException<ResponseDto<GetCanchaDto>>(guardada);

                var nuevas = urls.Select((url, indice) => new ImagenCancha
                {
                    IdCancha = guardada.Data.IdCancha,
                    UrlImagen = url,
                    Activo = true,
                    EsPrincipal = indice == entrada.IndicePrincipal
                }).ToArray();
                await imagenes.AddAsync(nuevas);
                await imagenes.SaveAsync();
                var detalle = await mediator.Send(new GetCanchaQuery(guardada.Data.IdCancha, true), token);
                if (detalle.IsValid && detalle.Data == null) detalle.AddErrorResult("No se pudo recuperar el detalle de la cancha.");
                if (!detalle.IsValid || detalle.Data == null) throw new ResultException<ResponseDto<GetCanchaDto>>(detalle);
                return detalle;
            }, null!, null!, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo crear la cancha con sus imágenes.");
            response = new ResponseDto<GetCanchaDto>();
            response.AddErrorResult("No se pudo completar la creación de la cancha.");
        }
        if (!response.IsValid) await EliminarArchivosAsync(claves);
        return response;
    }

    public async Task<ResponseDto<GetCanchaDto>> ActualizarAsync(int id, string datos, IFormFileCollection? archivos, CancellationToken ct)
    {
        var response = new ResponseDto<GetCanchaDto>();
        var nuevas = archivos?.ToArray() ?? Array.Empty<IFormFile>();
        ActualizarCanchaCompletaDto? solicitud;
        try { solicitud = datos.Length <= 131072 ? JsonSerializer.Deserialize<ActualizarCanchaCompletaDto>(datos, Json) : null; }
        catch (JsonException) { solicitud = null; }
        if (solicitud?.Cancha == null || id <= 0 || solicitud.Cancha.IdCancha != id || solicitud.Version == default || solicitud.ImagenesConservar == null ||
            solicitud.ImagenesConservar.Any(i => i <= 0) || solicitud.ImagenesConservar.Distinct().Count() != solicitud.ImagenesConservar.Count ||
            solicitud.ImagenesConservar.Count + nuevas.Length is < 1 or > 4)
        { response.AddErrorResult("Envía una cancha válida y entre una y cuatro imágenes finales."); return response; }
        if (solicitud.IdImagenPrincipal.HasValue == solicitud.IndicePrincipalNueva.HasValue ||
            solicitud.IdImagenPrincipal.HasValue && !solicitud.ImagenesConservar.Contains(solicitud.IdImagenPrincipal.Value) ||
            solicitud.IndicePrincipalNueva.HasValue && (solicitud.IndicePrincipalNueva < 0 || solicitud.IndicePrincipalNueva >= nuevas.Length))
        { response.AddErrorResult("Selecciona exactamente una imagen principal entre las que vas a conservar o agregar."); return response; }

        solicitud.Cancha.Version = solicitud.Version;
        var validacion = await updateValidator.ValidateAsync(new UpdateCanchaCommand(solicitud.Cancha), ct);
        if (!validacion.IsValid) { response.AddErrorResult(validacion); return response; }
        var actual = await canchas.GetByAsNoTrackingAsync(c => c.IdCancha == id && c.Activo, c => c.ImagenCancha.Where(i => i.Activo));
        if (actual == null || !await access.PuedeEditarCanchaAsync(actual) || actual.IdProveedor != solicitud.Cancha.IdProveedor)
        { response.AddErrorResult("No tienes permiso para actualizar esta cancha."); return response; }
        var error = ValidarEstadoActual(actual, solicitud);
        if (error != null) { response.AddErrorResult(error); return response; }
        if (!await SonImagenesValidasAsync(nuevas, ct))
        { response.AddErrorResult("Las imágenes nuevas deben ser JPEG, PNG o WebP, no vacías y de hasta 5 MB cada una."); return response; }

        var retiradas = actual.ImagenCancha.Where(i => !solicitud.ImagenesConservar.Contains(i.IdImagenCancha)).ToList();
        var clavesAntiguas = new List<string>();
        foreach (var imagen in retiradas)
        {
            var clave = ObtenerClaveStorage(imagen.UrlImagen);
            if (clave == null) { response.AddErrorResult("No se pudo identificar una imagen antigua en R2. Contacta con soporte antes de retirarla."); return response; }
            clavesAntiguas.Add(clave);
        }

        var clavesNuevas = new List<string>();
        try
        {
            var urls = await SubirImagenesAsync(actual.IdProveedor, nuevas, clavesNuevas, ct);
            ct.ThrowIfCancellationRequested();
            response = await unitOfWork.ExecuteInTransactionAsync(solicitud, async (entrada, token) =>
            {
                var cancha = await canchas.GetByAsync(c => c.IdCancha == id && c.Activo, c => c.ImagenCancha.Where(i => i.Activo));
                var errorActual = cancha == null ? "Cancha no encontrada." : ValidarEstadoActual(cancha, entrada);
                if (errorActual != null)
                {
                    var fallo = new ResponseDto<GetCanchaDto>(); fallo.AddErrorResult(errorActual);
                    throw new ResultException<ResponseDto<GetCanchaDto>>(fallo);
                }
                var guardada = await mediator.Send(new UpdateCanchaCommand(entrada.Cancha), token);
                if (guardada.IsValid && guardada.Data == null) guardada.AddErrorResult("No se pudo recuperar la cancha actualizada.");
                if (!guardada.IsValid || guardada.Data == null) throw new ResultException<ResponseDto<GetCanchaDto>>(guardada);

                foreach (var imagen in cancha!.ImagenCancha.Where(i => i.Activo).ToList())
                {
                    if (!entrada.ImagenesConservar.Contains(imagen.IdImagenCancha))
                    {
                        imagen.Activo = false;
                        imagen.EsPrincipal = false;
                        cancha.ImagenCancha.Remove(imagen);
                        await imagenes.DeleteAsync(imagen);
                    }
                    else
                    {
                        var esPrincipal = imagen.IdImagenCancha == entrada.IdImagenPrincipal;
                        if (imagen.EsPrincipal != esPrincipal)
                        {
                            imagen.EsPrincipal = esPrincipal;
                            await imagenes.UpdateAsync(imagen);
                        }
                    }
                }
                var agregadas = urls.Select((url, indice) => new ImagenCancha
                {
                    IdCancha = id, UrlImagen = url, Activo = true,
                    EsPrincipal = indice == entrada.IndicePrincipalNueva
                }).ToArray();
                if (agregadas.Length > 0) await imagenes.AddAsync(agregadas);
                await imagenes.SaveAsync();
                var detalle = await mediator.Send(new GetCanchaQuery(id, true), token);
                if (detalle.IsValid && detalle.Data == null) detalle.AddErrorResult("No se pudo recuperar el detalle actualizado.");
                if (!detalle.IsValid || detalle.Data == null) throw new ResultException<ResponseDto<GetCanchaDto>>(detalle);
                return detalle;
            }, null!, null!, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo actualizar la cancha {IdCancha} con sus imágenes.", id);
            response = new ResponseDto<GetCanchaDto>();
            response.AddErrorResult("No se pudo completar la actualización de la cancha. Recarga el detalle antes de guardar nuevamente.");
        }
        // Las fotos anteriores se retiran de R2 únicamente cuando SQL ya confirmó el cambio.
        if (response.IsValid) await EliminarArchivosAsync(clavesAntiguas);
        else await EliminarArchivosAsync(clavesNuevas);
        return response;
    }

    private static string? ValidarEstadoActual(Entity.Cancha cancha, ActualizarCanchaCompletaDto solicitud)
    {
        if ((cancha.UpdateDate ?? cancha.CreateDate) != solicitud.Version) 
            return "La cancha cambió. Recarga el detalle antes de guardar.";

        var ids = cancha.ImagenCancha.Where(i => i.Activo).Select(i => i.IdImagenCancha).ToHashSet();
        return solicitud.ImagenesConservar.All(ids.Contains) ? null : "Alguna imagen ya no existe o no pertenece a esta cancha. Recarga el detalle.";
    }

    private string? ObtenerClaveStorage(string url)
    {
        var prefijo = storage.GetPublicUrl("").TrimEnd('/') + "/";
        if (!url.StartsWith(prefijo, StringComparison.Ordinal) || !Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            uri.Query.Length > 0 || uri.Fragment.Length > 0) return null;
        var clave = url[prefijo.Length..].TrimStart('/');
        return clave.Length > 0 && !clave.Contains("..") ? clave : null;
    }

    private async Task<List<string>> SubirImagenesAsync(int proveedor, IEnumerable<IFormFile> archivos, List<string> claves, CancellationToken ct)
    {
        var urls = new List<string>();
        foreach (var archivo in archivos)
        {
            ct.ThrowIfCancellationRequested();
            var clave = $"canchas/proveedores/{proveedor}/imagen-{Guid.NewGuid():N}{Path.GetExtension(archivo.FileName).ToLowerInvariant()}";
            // Registra también el objeto cuyo upload pudo completarse sin devolver respuesta.
            claves.Add(clave);
            await using var stream = archivo.OpenReadStream();
            urls.Add(await storage.UploadAsync(stream, clave, archivo.ContentType));
        }
        return urls;
    }

    private async Task EliminarArchivosAsync(IEnumerable<string> claves)
    {
        foreach (var clave in claves.Distinct())
        {
            try { await storage.DeleteAsync(clave); }
            catch (Exception ex) { logger.LogWarning(ex, "No se pudo eliminar el archivo de R2: {Clave}", clave); }
        }
    }

    private static async Task<bool> SonImagenesValidasAsync(IEnumerable<IFormFile> archivos, CancellationToken ct)
    {
        foreach (var archivo in archivos) if (!await EsImagenValidaAsync(archivo, ct)) return false;
        return true;
    }

    private static async Task<bool> EsImagenValidaAsync(IFormFile archivo, CancellationToken ct)
    {
        if (archivo.Length is <= 0 or > 5 * 1024 * 1024) return false;
        await using var stream = archivo.OpenReadStream();
        var cabecera = new byte[12];
        var leidos = await stream.ReadAtLeastAsync(cabecera, 12, throwOnEndOfStream: false, ct);
        return Path.GetExtension(archivo.FileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => archivo.ContentType == "image/jpeg" && leidos >= 3 && cabecera[0] == 0xff && cabecera[1] == 0xd8 && cabecera[2] == 0xff,
            ".png" => archivo.ContentType == "image/png" && leidos >= 8 && cabecera.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".webp" => archivo.ContentType == "image/webp" && leidos >= 12 && Encoding.ASCII.GetString(cabecera, 0, 4) == "RIFF" && Encoding.ASCII.GetString(cabecera, 8, 4) == "WEBP",
            _ => false
        };
    }
}
