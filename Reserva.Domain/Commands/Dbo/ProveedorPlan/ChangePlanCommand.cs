using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class ChangePlanCommand : CommandBase<ChangePlanResponseDto>
    {
        public ChangePlanCommand(ChangePlanDto changePlanDto) => ChangePlanDto = changePlanDto;
        public ChangePlanDto ChangePlanDto { get; set; }
    }
}
