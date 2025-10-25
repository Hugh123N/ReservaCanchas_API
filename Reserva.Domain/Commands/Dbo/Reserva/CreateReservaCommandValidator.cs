using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Reserva
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
