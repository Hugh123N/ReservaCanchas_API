using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CreateProveedorPlanCommandValidator : CommandValidatorBase<CreateProveedorPlanCommand>
    {
        public CreateProveedorPlanCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
