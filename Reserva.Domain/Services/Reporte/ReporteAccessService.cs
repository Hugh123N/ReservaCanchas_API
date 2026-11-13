using Reserva.Common;
using Reserva.Dto.Reporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Services.Reporte;

public class ReporteAccessService
{
    private readonly IUserIdentity _identity;
    private readonly IRepository<Entity.Cancha> _canchas;
    private readonly IRepository<Entity.Operador> _operadores;
    private readonly IRepository<Entity.OperadorCancha> _asignaciones;

    public ReporteAccessService(IUserIdentity identity, IRepository<Entity.Cancha> canchas,
        IRepository<Entity.Operador> operadores, IRepository<Entity.OperadorCancha> asignaciones)
    {
        _identity = identity;
        _canchas = canchas;
        _operadores = operadores;
        _asignaciones = asignaciones;
    }

    public bool TieneRol(TipoReporte tipo) => _identity.GetUserRole() ==
        (tipo == TipoReporte.AgendaOperador ? Constants.Role.Operador : Constants.Role.Proveedor);

    public async Task<List<CanchaReporteDto>?> ObtenerCanchasAsync()
    {
        var usuario = _identity.GetCurrentUserId();
        var negocio = _identity.GetCurrentUserIdNegocio();
        if (!usuario.HasValue || usuario == Guid.Empty || negocio is null or <= 0) return null;
        IEnumerable<Entity.Cancha> canchas;
        if (_identity.GetUserRole() == Constants.Role.Proveedor)
        {
            canchas = await _canchas.FindByAsNoTrackingAsync(c => c.IdProveedor == negocio);
        }
        else if (_identity.GetUserRole() == Constants.Role.Operador)
        {
            var idUsuario = usuario.Value.ToString();
            var operador = await _operadores.GetByAsNoTrackingAsync(o => o.Activo && o.IdOperador == negocio && o.IdUsuario == idUsuario);
            if (operador == null) return null;
            var ids = (await _asignaciones.FindByAsNoTrackingAsync(a => a.Activo && a.IdOperador == operador.IdOperador))
                .Select(a => a.IdCancha).Distinct().ToList();
            canchas = await _canchas.FindByAsNoTrackingAsync(c => c.IdProveedor == operador.IdProveedor && ids.Contains(c.IdCancha));
        }
        else return null;
        return canchas.OrderBy(c => c.Nombre).Select(c => new CanchaReporteDto(c.IdCancha,
            c.Nombre + (c.Activo ? "" : " (inactiva)"), string.IsNullOrWhiteSpace(c.ZonaHoraria) ? "America/Lima" : c.ZonaHoraria, c.Activo)).ToList();
    }
}
