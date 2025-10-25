using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.MetodoPago
{
    public class CreateMetodoPagoCommandValidator : CommandValidatorBase<CreateMetodoPagoCommand>
    {
        public CreateMetodoPagoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
