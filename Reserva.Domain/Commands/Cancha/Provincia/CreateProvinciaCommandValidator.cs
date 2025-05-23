using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class CreateProvinciaCommandValidator : CommandValidatorBase<CreateProvinciaCommand>
    {
        public CreateProvinciaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
