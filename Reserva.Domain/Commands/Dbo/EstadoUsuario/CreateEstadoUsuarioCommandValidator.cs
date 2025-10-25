using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoUsuario
{
    public class CreateEstadoUsuarioCommandValidator : CommandValidatorBase<CreateEstadoUsuarioCommand>
    {
        public CreateEstadoUsuarioCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
