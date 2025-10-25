using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.GananciaProveedor
{
    public class CreateGananciaProveedorCommandValidator : CommandValidatorBase<CreateGananciaProveedorCommand>
    {
        public CreateGananciaProveedorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
