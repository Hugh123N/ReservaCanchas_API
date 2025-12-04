using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class CreateHorarioCanchaCommandValidator : CommandValidatorBase<CreateHorarioCanchaCommand>
    {
        public CreateHorarioCanchaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
