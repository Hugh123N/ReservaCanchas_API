using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class DeleteMetodoPagoCommand : CommandBase
    {
        public DeleteMetodoPagoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
