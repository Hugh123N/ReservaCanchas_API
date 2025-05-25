using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class DeleteEstadoPagoCommand : CommandBase
    {
        public DeleteEstadoPagoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
