using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoPago
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
