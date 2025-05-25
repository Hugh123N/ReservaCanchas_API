using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class CreateDiaSemanaCommandValidator : CommandValidatorBase<CreateDiaSemanaCommand>
    {
        public CreateDiaSemanaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
