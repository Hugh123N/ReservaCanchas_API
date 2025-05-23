using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class CreateDetallePagoCommandValidator : CommandValidatorBase<CreateDetallePagoCommand>
    {
        public CreateDetallePagoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
