using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.CanchaFavorita;

namespace Reserva.Domain.Commands.Dbo.CanchaFavorita
{
    public class CreateCanchaFavoritaCommand : CommandBase<GetCanchaFavoritaDto>
    {
        public CreateCanchaFavoritaCommand(CreateCanchaFavoritaDto createDto) => CreateDto = createDto;
        public CreateCanchaFavoritaDto CreateDto { get; set; }
    }
}
