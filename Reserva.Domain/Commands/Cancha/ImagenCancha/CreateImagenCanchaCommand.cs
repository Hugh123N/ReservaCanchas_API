using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class CreateImagenCanchaCommand : CommandBase<GetImagenCanchaDto>
    {
        public CreateImagenCanchaCommand(CreateImagenCanchaDto createDto) => CreateDto = createDto;
        public CreateImagenCanchaDto CreateDto { get; set; }
    }
}
