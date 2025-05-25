using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Pago
{
    public class DeletePagoCommand : CommandBase
    {
        public DeletePagoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
