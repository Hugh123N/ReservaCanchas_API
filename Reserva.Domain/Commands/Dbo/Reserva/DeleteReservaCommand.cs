using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class DeleteReservaCommand : CommandBase
    {
        public DeleteReservaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
