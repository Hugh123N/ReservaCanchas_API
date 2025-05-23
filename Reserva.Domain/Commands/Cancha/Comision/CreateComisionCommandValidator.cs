using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class CreateComisionCommandValidator : CommandValidatorBase<CreateComisionCommand>
    {
        public CreateComisionCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
