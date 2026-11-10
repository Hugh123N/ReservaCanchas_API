using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class CreateProveedorCommandValidator : CommandValidatorBase<CreateProveedorCommand>
    {
        public CreateProveedorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RequiredString(x => x.CreateDto.TipoDoc, "TipoDoc", null, 20);
                MaximumLength(x => x.CreateDto.NroDoc, "NroDoc", 20);
            });
        }
    }
}
