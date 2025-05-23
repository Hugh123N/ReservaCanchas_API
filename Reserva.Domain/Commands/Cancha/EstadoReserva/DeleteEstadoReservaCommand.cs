using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class DeleteEstadoReservaCommand : CommandBase
    {
        public DeleteEstadoReservaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
