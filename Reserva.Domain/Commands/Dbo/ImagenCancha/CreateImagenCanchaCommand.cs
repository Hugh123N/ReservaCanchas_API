using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class CreateImagenCanchaCommand : CommandBase<GetImagenCanchaDto>
    {
        public CreateImagenCanchaCommand(CreateImagenCanchaDto createDto) => CreateDto = createDto;
        public CreateImagenCanchaDto CreateDto { get; set; }
    }
}
