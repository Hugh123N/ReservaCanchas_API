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

public class CrearCanchaCompletaService(CreateCanchaCommandValidator validator, NegocioAccessService access,
    IPlanLimitValidationService limites, IStorageService storage, IRepository<ImagenCancha> imagenes,
    IUnitOfWork unitOfWork, IMediator mediator, ILogger<CrearCanchaCompletaService> logger)
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
        foreach (var archivo in archivos)
        {
            if (!await EsImagenValidaAsync(archivo, ct))
            { response.AddErrorResult("Las imágenes deben ser JPEG, PNG o WebP, no vacías y de hasta 5 MB cada una."); return response; }
        }

        var claves = new List<string>();
        var urls = new List<string>();
        try
        {
            foreach (var archivo in archivos)
            {
                ct.ThrowIfCancellationRequested();
                var clave = $"canchas/proveedores/{proveedor.IdProveedor}/imagen-{Guid.NewGuid():N}{Path.GetExtension(archivo.FileName).ToLowerInvariant()}";
                // Permite borrar también el objeto si R2 lo recibió pero se perdió su respuesta.
                claves.Add(clave);
                await using var stream = archivo.OpenReadStream();
                urls.Add(await storage.UploadAsync(stream, clave, archivo.ContentType));
            }
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

    private async Task EliminarArchivosAsync(IEnumerable<string> claves)
    {
        foreach (var clave in claves)
        {
            try { await storage.DeleteAsync(clave); }
            catch (Exception ex) { logger.LogWarning(ex, "No se pudo eliminar el archivo de una creación fallida: {Clave}", clave); }
        }
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
