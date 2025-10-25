using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoCancha
{
    public class CreateEstadoCanchaCommandValidator : CommandValidatorBase<CreateEstadoCanchaCommand>
    {
        public CreateEstadoCanchaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
