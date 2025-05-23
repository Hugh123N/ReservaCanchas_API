using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class CreateRolCommandValidator : CommandValidatorBase<CreateRolCommand>
    {
        public CreateRolCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
