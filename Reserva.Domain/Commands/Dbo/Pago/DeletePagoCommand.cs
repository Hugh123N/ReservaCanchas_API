using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class DeletePagoCommand : CommandBase
    {
        public DeletePagoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
