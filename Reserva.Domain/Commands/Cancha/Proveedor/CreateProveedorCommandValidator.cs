using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class CreateProveedorCommandValidator : CommandValidatorBase<CreateProveedorCommand>
    {
        public CreateProveedorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
