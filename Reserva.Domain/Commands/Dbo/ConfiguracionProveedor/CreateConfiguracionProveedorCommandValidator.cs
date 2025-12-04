using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class CreateConfiguracionProveedorCommandValidator : CommandValidatorBase<CreateConfiguracionProveedorCommand>
    {
        public CreateConfiguracionProveedorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
