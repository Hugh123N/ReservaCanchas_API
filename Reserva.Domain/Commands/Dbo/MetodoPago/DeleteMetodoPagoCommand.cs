using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.MetodoPago
{
    public class DeleteMetodoPagoCommand : CommandBase
    {
        public DeleteMetodoPagoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
