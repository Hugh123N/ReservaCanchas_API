using Reserva.Common;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Services.Pago;

public class PagoReservaAccessService
{
    private readonly IUserIdentity _identity;
    private readonly IRepository<Entity.Operador> _operadores;
    private readonly IRepository<Entity.OperadorCancha> _asignaciones;

    public PagoReservaAccessService(IUserIdentity identity, IRepository<Entity.Operador> operadores,
        IRepository<Entity.OperadorCancha> asignaciones)
    {
        _identity = identity;
        _operadores = operadores;
        _asignaciones = asignaciones;
    }

    public bool EsProveedor => _identity.GetUserRole() == Constants.Role.Proveedor;

    public async Task<bool> PuedeAccederAsync(Entity.Reserva reserva)
    {
        var cancha = reserva.IdCanchaNavigation;
        var idUsuario = _identity.GetCurrentUserId();

        if (!reserva.Activo || cancha == null || !idUsuario.HasValue || idUsuario == Guid.Empty) 
            return false;

        var negocio = _identity.GetCurrentUserIdNegocio();
        if (EsProveedor) 
            return cancha.IdProveedor == negocio;

        if (_identity.GetUserRole() != Constants.Role.Operador) 
            return false;

        var usuario = idUsuario.Value.ToString();
        var operador = await _operadores.GetByAsync(o => o.IdOperador == negocio && o.Activo &&
            o.IdUsuario == usuario && o.IdProveedor == cancha.IdProveedor);

        if (operador == null) 
            return false;

        return await _asignaciones.GetByAsync(a => a.IdOperador == operador.IdOperador &&
            a.IdCancha == cancha.IdCancha && a.Activo) != null;
    }

    public bool PuedeOperarPorFecha(Entity.Reserva reserva)
    {
        if (EsProveedor) 
            return true;

        var zona = TimezoneUtils.ObtenerZonaHoraria(reserva.IdCanchaNavigation.ZonaHoraria ?? "America/Lima");
        return reserva.FechaReserva.Date >= DateTimeHelper.ObtenerAhoraLocal(zona).Date;
    }
}
