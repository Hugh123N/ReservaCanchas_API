using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class CreateCanchaFavoritaCommandValidator : CommandValidatorBase<CreateCanchaFavoritaCommand>
    {
        public CreateCanchaFavoritaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
