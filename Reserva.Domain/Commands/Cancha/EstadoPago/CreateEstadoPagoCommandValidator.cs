using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class CreateEstadoPagoCommandValidator : CommandValidatorBase<CreateEstadoPagoCommand>
    {
        public CreateEstadoPagoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
