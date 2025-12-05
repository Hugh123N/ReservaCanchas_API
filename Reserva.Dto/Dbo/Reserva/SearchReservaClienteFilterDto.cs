namespace Reserva.Dto.Dbo.Reserva
{
    public class SearchReservaClienteFilterDto
    {
        public string? CodigoEstado { get; set; }

        public DateTimeOffset? FechaDesde { get; set; }

        public DateTimeOffset? FechaHasta { get; set; }

        public string? EstadoPago { get; set; }

        public string? CodigoReserva { get; set; }

        public string? NombreCancha { get; set; }
    }
}
