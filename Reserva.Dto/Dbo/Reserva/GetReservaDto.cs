using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Dto.Dbo.EstadoReserva;

namespace Reserva.Dto.Dbo.Reserva
{
    public class GetReservaDto : ReservaDto
    {
        public int IdReserva { get; set; }
        public string? CodigoReserva { get; set; }

        public string NombreCancha { get; set; } = null!;
        public string DireccionCancha { get; set; } = null!;
        public string? TelefonoCancha { get; set; }

        public string NombreCliente { get; set; } = null!;
        public string? NumeroCliente { get; set; }
        public string? EmailCliente { get; set; }

        public string? NombreOperadorConfirmo { get; set; }
        public string? NombreDeporte { get; set; }
        public DateTimeOffset FechaCreacion { get; set; }
        public DateTimeOffset? FechaModificacion { get; set; }

        public List<GetDetalleReservaDto> Horarios { get; set; } = new();
        public GetPagoDto? PagoReserva { get; set; }
        public GetConfiguracionProveedorDto? ConfiguracionProveedor { get; set; }
        public GetEstadoReservaDto? EstadoReserva { get; set; }
    }
}
