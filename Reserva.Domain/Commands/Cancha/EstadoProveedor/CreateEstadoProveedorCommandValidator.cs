using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class CreateEstadoProveedorCommandValidator : CommandValidatorBase<CreateEstadoProveedorCommand>
    {
        public CreateEstadoProveedorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
