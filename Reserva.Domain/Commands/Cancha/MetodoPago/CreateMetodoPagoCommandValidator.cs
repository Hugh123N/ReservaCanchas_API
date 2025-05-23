using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
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
