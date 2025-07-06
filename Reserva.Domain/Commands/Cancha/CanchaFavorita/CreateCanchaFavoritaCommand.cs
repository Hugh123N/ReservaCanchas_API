using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class CreateCanchaFavoritaCommand : CommandBase<GetCanchaFavoritaDto>
    {
        public CreateCanchaFavoritaCommand(CreateCanchaFavoritaDto createDto) => CreateDto = createDto;
        public CreateCanchaFavoritaDto CreateDto { get; set; }
    }
}
