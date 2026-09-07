using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CheckoutPlanCommand : CommandBase<int>
    {
        public CheckoutPlanCommand(CheckoutPlanDto checkoutDto) => CheckoutDto = checkoutDto;
        public CheckoutPlanDto CheckoutDto { get; set; }
    }
}