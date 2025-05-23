using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class UpdateImagenCanchaCommand : CommandBase<GetImagenCanchaDto>
    {
        public UpdateImagenCanchaCommand(UpdateImagenCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateImagenCanchaDto UpdateDto { get; set; }
    }
}
