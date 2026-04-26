using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class UpdatePlanLimiteCommand : CommandBase<GetPlanLimiteDto>
    {
        public UpdatePlanLimiteCommand(UpdatePlanLimiteDto updateDto) => UpdateDto = updateDto;
        public UpdatePlanLimiteDto UpdateDto { get; set; }
    }
}
