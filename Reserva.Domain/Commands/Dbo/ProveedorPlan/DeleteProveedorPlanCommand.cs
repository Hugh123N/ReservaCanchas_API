using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class DeleteProveedorPlanCommand : CommandBase
    {
        public DeleteProveedorPlanCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
