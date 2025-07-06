using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class UpdateCanchaFavoritaCommand : CommandBase<GetCanchaFavoritaDto>
    {
        public UpdateCanchaFavoritaCommand(UpdateCanchaFavoritaDto updateDto) => UpdateDto = updateDto;
        public UpdateCanchaFavoritaDto UpdateDto { get; set; }
    }
}
