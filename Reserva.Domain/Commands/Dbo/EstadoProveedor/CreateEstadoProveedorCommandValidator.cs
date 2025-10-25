using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoProveedor
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
