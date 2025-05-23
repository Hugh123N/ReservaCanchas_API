using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class CreateZonaCommandValidator : CommandValidatorBase<CreateZonaCommand>
    {
        public CreateZonaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
