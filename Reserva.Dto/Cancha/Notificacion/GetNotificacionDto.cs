namespace Reserva.Dto.Cancha.Notificacion
{
    public class GetNotificacionDto : NotificacionDto
    {
        public int IdNotificacion { get; set; }
        public bool Activo { get; set; }
    }
}
