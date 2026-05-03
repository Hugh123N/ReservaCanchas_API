using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class RetryPaymentPlanCommand : CommandBase
    {
        public RetryPaymentPlanCommand(RetryPaymentDto retryPaymentDto) => RetryPaymentDto = retryPaymentDto;
        public RetryPaymentDto RetryPaymentDto { get; set; }
    }
}