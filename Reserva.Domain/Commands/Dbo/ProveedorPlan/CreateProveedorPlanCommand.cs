using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CreateProveedorPlanCommand : CommandBase<GetProveedorPlanDto>
    {
        public CreateProveedorPlanCommand(CreateProveedorPlanDto createDto) => CreateDto = createDto;
        public CreateProveedorPlanDto CreateDto { get; set; }
    }
}
