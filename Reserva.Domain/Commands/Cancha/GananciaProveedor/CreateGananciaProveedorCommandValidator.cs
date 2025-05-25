using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
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
