using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class DeletePagoPlanCommand : CommandBase
    {
        public DeletePagoPlanCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
