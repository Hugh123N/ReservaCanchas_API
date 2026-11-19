using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.GestionAdmin;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services.GestionAdmin;

public class GestionAdminService(IRepository<Entity.Proveedor> proveedores, IRepository<Entity.ProveedorPlan> planes,
    IRepository<Entity.Cancha> canchas, IRepository<Entity.Reserva> reservas, IRepository<Entity.EstadoProveedor> estadosProveedor,
    IRepository<Entity.EstadoCancha> estadosCancha, NegocioAccessService access, CancelarRenovacionService renovaciones)
{
    public async Task<ResponseDto<PaginaGestionAdminDto<ProveedorAdminDto>>> ProveedoresAsync(FiltroGestionAdminDto filtro, CancellationToken ct)
    {
        var response = new ResponseDto<PaginaGestionAdminDto<ProveedorAdminDto>>();
        if (!ValidarFiltro(filtro, response)) return response;
        var query = GestionAdminConsultas.Proveedores(proveedores.FindAllAsNoTracking(), planes.FindAllAsNoTracking(), reservas.FindAllAsNoTracking(), DateTimeOffset.UtcNow);
        if (!string.IsNullOrWhiteSpace(filtro.Texto))
        {
            var texto = filtro.Texto.Trim();
            query = query.Where(p => p.Nombre.Contains(texto) || p.Titular.Contains(texto) || p.Email != null && p.Email.Contains(texto) ||
                p.Documento != null && p.Documento.Contains(texto) || p.Ruc != null && p.Ruc.Contains(texto));
        }
        if (!string.IsNullOrEmpty(filtro.Estado)) query = query.Where(p => p.Estado == filtro.Estado);
        response.UpdateData(new() { Total = await query.CountAsync(ct), Pagina = filtro.Pagina, TamanoPagina = filtro.TamanoPagina,
            Items = await query.OrderByDescending(p => p.Registro).ThenByDescending(p => p.IdProveedor)
                .Skip((filtro.Pagina - 1) * filtro.TamanoPagina).Take(filtro.TamanoPagina).ToListAsync(ct) });
        return response;
    }

    public async Task<ResponseDto<ProveedorAdminDto>> ProveedorAsync(int id, CancellationToken ct)
    {
        var response = new ResponseDto<ProveedorAdminDto>();
        if (!ValidarAdmin(response)) return response;
        var dato = await GestionAdminConsultas.Proveedores(proveedores.FindAllAsNoTracking(), planes.FindAllAsNoTracking(), reservas.FindAllAsNoTracking(), DateTimeOffset.UtcNow)
            .SingleOrDefaultAsync(p => p.IdProveedor == id, ct);
        if (dato == null) response.AddErrorResult("Proveedor no encontrado."); else response.UpdateData(dato);
        return response;
    }

    public async Task<ResponseDto<PaginaGestionAdminDto<CanchaAdminDto>>> CanchasAsync(FiltroGestionAdminDto filtro, CancellationToken ct)
    {
        var response = new ResponseDto<PaginaGestionAdminDto<CanchaAdminDto>>();
        if (!ValidarFiltro(filtro, response)) return response;
        var query = GestionAdminConsultas.Canchas(canchas.FindAllAsNoTracking());
        if (!string.IsNullOrWhiteSpace(filtro.Texto))
        {
            var texto = filtro.Texto.Trim();
            query = query.Where(c => c.Nombre.Contains(texto) || c.Proveedor.Contains(texto) || c.Direccion != null && c.Direccion.Contains(texto));
        }
        if (!string.IsNullOrEmpty(filtro.Estado)) query = query.Where(c => c.Estado == filtro.Estado);
        if (filtro.IdProveedor.HasValue) query = query.Where(c => c.IdProveedor == filtro.IdProveedor);
        response.UpdateData(new() { Total = await query.CountAsync(ct), Pagina = filtro.Pagina, TamanoPagina = filtro.TamanoPagina,
            Items = await query.OrderBy(c => c.Registro).ThenBy(c => c.IdCancha)
                .Skip((filtro.Pagina - 1) * filtro.TamanoPagina).Take(filtro.TamanoPagina).ToListAsync(ct) });
        return response;
    }

    public async Task<ResponseDto> CambiarProveedorAsync(int id, CambiarEstadoGestionDto cambio)
    {
        var response = new ResponseDto();
        if (!ValidarAdmin(response)) return response;
        if (id <= 0 || cambio.Version == default || cambio.Estado is not (NegocioReglas.Activo or NegocioReglas.Suspendido or NegocioReglas.Inactivo))
        { response.AddErrorResult("El proveedor, la versión o el estado solicitado no son válidos."); return response; }
        var proveedor = await proveedores.GetByAsync(p => p.IdProveedor == id, p => p.IdEstadoProveedorNavigation);
        if (proveedor == null) { response.AddErrorResult("Proveedor no encontrado."); return response; }
        if ((proveedor.UpdateDate ?? proveedor.CreateDate) != cambio.Version)
        { response.AddErrorResult("El proveedor cambió. Recarga su detalle antes de continuar."); return response; }
        if (NegocioReglas.Estado(proveedor) == cambio.Estado) { response.AddOkResult("El proveedor ya tiene ese estado."); return response; }
        var codigo = cambio.Estado == NegocioReglas.Suspendido ? Constants.ESTADO_PROVEEDOR.Suspendido : Constants.ESTADO_PROVEEDOR.Aprobado;
        var estado = await estadosProveedor.GetByAsNoTrackingAsync(e => e.Codigo == codigo && e.Activo);
        var pendiente = await estadosCancha.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_CANCHA.Pendiente && e.Activo);
        if (estado == null || pendiente == null)
        { response.AddErrorResult("Falta configurar los estados de gestión. Ejecuta el script de estados de Admin."); return response; }

        if (cambio.Estado != NegocioReglas.Activo)
        {
            var suscripciones = await planes.FindByAsync(p => p.IdProveedor == id && p.Activo);
            var cancelacion = await renovaciones.CancelarAsync(suscripciones, "Renovación cancelada por restricción administrativa del proveedor.");
            if (!cancelacion.IsValid) { response.AttachResults(cancelacion); return response; }
            foreach (var cancha in await canchas.FindByAsync(c => c.IdProveedor == id && c.Activo))
            {
                cancha.IdEstadoCancha = pendiente.IdEstadoCancha;
                await canchas.UpdateAsync(cancha);
            }
        }
        proveedor.Activo = cambio.Estado != NegocioReglas.Inactivo;
        proveedor.IdEstadoProveedor = estado.IdEstadoProveedor;
        await proveedores.UpdateAsync(proveedor);
        response.AddOkResult(cambio.Estado == NegocioReglas.Activo
            ? "Proveedor reactivado. Las canchas siguen pendientes y la renovación automática no se reactiva."
            : "Estado actualizado. Canchas pendientes de revisión y renovación automática cancelada. Se conservan las reservas y pagos existentes.");
        return response;
    }

    public async Task<ResponseDto> CambiarCanchaAsync(int id, CambiarEstadoGestionDto cambio, bool reenvio = false)
    {
        var response = new ResponseDto();
        var cancha = await canchas.GetByAsync(c => c.IdCancha == id && c.Activo, c => c.IdEstadoCanchaNavigation,
            c => c.IdProveedorNavigation.IdEstadoProveedorNavigation);
        if (cancha == null) { response.AddErrorResult("Cancha no encontrada."); return response; }
        if (reenvio)
        {
            if (!await access.PuedeEditarCanchaAsync(cancha)) { response.AddErrorResult("No tienes permiso para reenviar esta cancha."); return response; }
            if (cancha.IdEstadoCanchaNavigation.Codigo != Constants.ESTADO_CANCHA.Rechazado || cambio.Estado != Constants.ESTADO_CANCHA.Pendiente)
            { response.AddErrorResult("Solo una cancha rechazada puede reenviarse a revisión."); return response; }
        }
        else if (!ValidarAdmin(response)) return response;
        if (cambio.Version == default || (cancha.UpdateDate ?? cancha.CreateDate) != cambio.Version)
        { response.AddErrorResult("La cancha cambió. Recarga el detalle antes de continuar."); return response; }
        var actual = cancha.IdEstadoCanchaNavigation.Codigo;
        var permitido = reenvio || actual == Constants.ESTADO_CANCHA.Pendiente &&
            (cambio.Estado == Constants.ESTADO_CANCHA.Aprobado || cambio.Estado == Constants.ESTADO_CANCHA.Rechazado) ||
            actual == Constants.ESTADO_CANCHA.Aprobado && cambio.Estado == Constants.ESTADO_CANCHA.Suspendido ||
            actual == Constants.ESTADO_CANCHA.Suspendido && cambio.Estado == Constants.ESTADO_CANCHA.Pendiente;
        if (!permitido) { response.AddErrorResult("Ese cambio de estado no está permitido. Revisa el estado actual de la cancha."); return response; }
        if (cambio.Estado == Constants.ESTADO_CANCHA.Aprobado && !NegocioReglas.Habilitado(cancha.IdProveedorNavigation))
        { response.AddErrorResult("Reactiva primero al proveedor para aprobar su cancha."); return response; }
        var estado = await estadosCancha.GetByAsNoTrackingAsync(e => e.Codigo == cambio.Estado && e.Activo);
        if (estado == null) { response.AddErrorResult("Estado de cancha no configurado."); return response; }
        cancha.IdEstadoCancha = estado.IdEstadoCancha;
        await canchas.UpdateAsync(cancha);
        response.AddOkResult(reenvio ? "Cancha reenviada a revisión. Puedes seguir gestionando tus reservas." : "Estado de publicación actualizado.");
        return response;
    }

    private bool ValidarAdmin(ResponseDto response)
    {
        if (access.EsAdmin) return true;
        response.AddErrorResult("Esta gestión es exclusiva del administrador.");
        return false;
    }

    private bool ValidarFiltro(FiltroGestionAdminDto filtro, ResponseDto response)
    {
        if (!ValidarAdmin(response)) return false;
        if (filtro.Pagina is < 1 or > 100000 || filtro.TamanoPagina is < 1 or > 100 || filtro.Texto?.Length > 150 || filtro.Estado?.Length > 20 || filtro.IdProveedor is <= 0)
        { response.AddErrorResult("Los filtros o la paginación no son válidos."); return false; }
        return true;
    }
}
