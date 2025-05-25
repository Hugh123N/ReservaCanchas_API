namespace Reserva.Dto.Cancha.Ubigeo
{
    public class SelectUbigeoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public string? IdUbigeo { get; set; }
        public bool? Activo { get; set; }
    }
}
