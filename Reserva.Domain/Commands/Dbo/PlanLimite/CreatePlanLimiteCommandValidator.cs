using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class CreatePlanLimiteCommandValidator : CommandValidatorBase<CreatePlanLimiteCommand>
    {
        public CreatePlanLimiteCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
