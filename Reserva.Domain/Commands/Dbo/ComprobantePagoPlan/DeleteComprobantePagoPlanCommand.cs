using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class DeleteComprobantePagoPlanCommand : CommandBase
    {
        public DeleteComprobantePagoPlanCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
