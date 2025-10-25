using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoReserva
{
    public class CreateEstadoReservaCommandValidator : CommandValidatorBase<CreateEstadoReservaCommand>
    {
        public CreateEstadoReservaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
