using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class CreateCanchaCommandValidator : CommandValidatorBase<CreateCanchaCommand>
    {
        public CreateCanchaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
