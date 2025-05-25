namespace Reserva.Dto.Cancha.ImagenCancha
{
    public class SelectImagenCanchaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdImagenCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
