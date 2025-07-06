using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class CreateIntentoLoginCommandValidator : CommandValidatorBase<CreateIntentoLoginCommand>
    {
        public CreateIntentoLoginCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
