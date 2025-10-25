namespace Reserva.Dto.Dbo.EstadoReserva
{
    public class SelectEstadoReservaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoReserva { get; set; }
        public bool? Activo { get; set; }
    }
}
