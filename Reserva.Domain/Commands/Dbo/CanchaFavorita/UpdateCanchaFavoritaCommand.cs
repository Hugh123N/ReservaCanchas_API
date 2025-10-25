using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.CanchaFavorita;

namespace Reserva.Domain.Commands.Dbo.CanchaFavorita
{
    public class UpdateCanchaFavoritaCommand : CommandBase<GetCanchaFavoritaDto>
    {
        public UpdateCanchaFavoritaCommand(UpdateCanchaFavoritaDto updateDto) => UpdateDto = updateDto;
        public UpdateCanchaFavoritaDto UpdateDto { get; set; }
    }
}
