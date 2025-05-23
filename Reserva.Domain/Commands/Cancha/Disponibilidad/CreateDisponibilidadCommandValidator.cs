using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class CreateDisponibilidadCommandValidator : CommandValidatorBase<CreateDisponibilidadCommand>
    {
        public CreateDisponibilidadCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
