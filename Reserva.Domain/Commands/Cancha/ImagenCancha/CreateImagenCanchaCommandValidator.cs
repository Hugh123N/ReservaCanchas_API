using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class CreateImagenCanchaCommandValidator : CommandValidatorBase<CreateImagenCanchaCommand>
    {
        public CreateImagenCanchaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
