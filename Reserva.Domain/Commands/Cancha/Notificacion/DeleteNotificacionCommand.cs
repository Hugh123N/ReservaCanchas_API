using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class DeleteNotificacionCommand : CommandBase
    {
        public DeleteNotificacionCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
