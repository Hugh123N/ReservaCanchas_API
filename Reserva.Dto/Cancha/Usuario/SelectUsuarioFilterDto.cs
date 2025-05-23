namespace Reserva.Dto.Cancha.Usuario
{
    public class SelectUsuarioFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdUsuario { get; set; }
        public bool? Activo { get; set; }
    }
}
