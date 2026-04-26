using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class CreateComprobantePagoPlanCommandValidator : CommandValidatorBase<CreateComprobantePagoPlanCommand>
    {
        public CreateComprobantePagoPlanCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
