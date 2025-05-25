using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class CreateReservaCommandValidator : CommandValidatorBase<CreateReservaCommand>
    {
        public CreateReservaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
