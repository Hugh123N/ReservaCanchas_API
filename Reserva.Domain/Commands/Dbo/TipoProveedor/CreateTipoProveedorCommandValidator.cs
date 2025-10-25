using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoProveedor
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
