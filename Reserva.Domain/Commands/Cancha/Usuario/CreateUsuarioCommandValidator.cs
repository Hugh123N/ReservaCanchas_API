using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateUsuarioCommandValidator : CommandValidatorBase<CreateUsuarioCommand>
    {
        public CreateUsuarioCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
