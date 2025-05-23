using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class CreateDistritoCommandValidator : CommandValidatorBase<CreateDistritoCommand>
    {
        public CreateDistritoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
