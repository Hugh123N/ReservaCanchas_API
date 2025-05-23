namespace Reserva.Dto.Cancha.Rol
{
    public class SelectRolFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdRol { get; set; }
        public bool? Activo { get; set; }
    }
}
