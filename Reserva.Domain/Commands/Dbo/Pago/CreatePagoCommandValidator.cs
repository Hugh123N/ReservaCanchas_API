using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class CreatePagoCommandValidator : CommandValidatorBase<CreatePagoCommand>
    {
        public CreatePagoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
