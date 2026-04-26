using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class UpdateProveedorPlanCommand : CommandBase<GetProveedorPlanDto>
    {
        public UpdateProveedorPlanCommand(UpdateProveedorPlanDto updateDto) => UpdateDto = updateDto;
        public UpdateProveedorPlanDto UpdateDto { get; set; }
    }
}
