using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.DetallePago
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
