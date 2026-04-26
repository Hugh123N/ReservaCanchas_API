using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class CreatePlanLimiteCommand : CommandBase<GetPlanLimiteDto>
    {
        public CreatePlanLimiteCommand(CreatePlanLimiteDto createDto) => CreateDto = createDto;
        public CreatePlanLimiteDto CreateDto { get; set; }
    }
}
