namespace Reserva.Dto.Dbo.Notificacion
{
    public class GetNotificacionDto : NotificacionDto
    {
        public int IdNotificacion { get; set; }
        public bool Activo { get; set; }
    }
}
