using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class CreateTipoProveedorCommandValidator : CommandValidatorBase<CreateTipoProveedorCommand>
    {
        public CreateTipoProveedorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
