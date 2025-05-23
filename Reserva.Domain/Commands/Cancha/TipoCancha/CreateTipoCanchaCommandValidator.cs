using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class CreateTipoCanchaCommandValidator : CommandValidatorBase<CreateTipoCanchaCommand>
    {
        public CreateTipoCanchaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
