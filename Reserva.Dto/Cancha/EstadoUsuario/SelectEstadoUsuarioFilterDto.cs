namespace Reserva.Dto.Cancha.EstadoUsuario
{
    public class SelectEstadoUsuarioFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoUsuario { get; set; }
        public bool? Activo { get; set; }
    }
}
