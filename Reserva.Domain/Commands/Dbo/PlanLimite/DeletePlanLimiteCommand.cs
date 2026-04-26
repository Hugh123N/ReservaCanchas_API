using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class DeletePlanLimiteCommand : CommandBase
    {
        public DeletePlanLimiteCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
