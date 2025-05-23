namespace Reserva.Dto.Cancha.Rol
{
    public class SearchRolFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdRol { get; set; }
        public bool? Activo { get; set; }
    }
}
