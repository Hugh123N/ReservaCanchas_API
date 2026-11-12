using AutoMapper;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Helpers;

public static class ReservaDetalleDtoMapper
{
    public static GetReservaDto Map(IMapper mapper, Entity.Reserva reserva,
        IEnumerable<Entity.DetalleReserva> detalles, Entity.Pago? pago, Entity.ConfiguracionProveedor? configuracion)
    {
        var dto = mapper.Map<GetReservaDto>(reserva);
        dto.CodigoReserva = reserva.CodigoReserva;
        dto.NombreCancha = reserva.IdCanchaNavigation?.Nombre ?? string.Empty;
        dto.DireccionCancha = reserva.IdCanchaNavigation?.Direccion ?? string.Empty;
        dto.TelefonoCancha = reserva.IdCanchaNavigation?.TelefonoCancha;
        dto.NombreCliente = $"{reserva.IdClienteNavigation?.Nombres} {reserva.IdClienteNavigation?.Apellidos}".Trim();
        dto.NumeroCliente = reserva.IdClienteNavigation?.Telefono;
        dto.EmailCliente = reserva.IdClienteNavigation?.Email;
        dto.NombreOperadorConfirmo = reserva.IdOperadorConfirmoNavigation == null ? null :
            $"{reserva.IdOperadorConfirmoNavigation.Nombres} {reserva.IdOperadorConfirmoNavigation.Apellidos}";
        dto.NombreDeporte = reserva.IdTipoDeporteNavigation?.Nombre;
        dto.UserCreateReserva = reserva.UserNameCreate;
        dto.FechaCreacion = reserva.CreateDate;
        dto.FechaModificacion = reserva.UpdateDate;
        dto.Horarios = mapper.Map<List<GetDetalleReservaDto>>(detalles.Where(d => d.Activo).ToList()) ?? new();
        dto.PagoReserva = mapper.Map<GetPagoDto>(pago);
        dto.ConfiguracionProveedor = mapper.Map<GetConfiguracionProveedorDto>(configuracion);
        return dto;
    }
}
