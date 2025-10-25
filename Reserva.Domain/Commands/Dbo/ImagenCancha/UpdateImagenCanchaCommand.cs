using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class UpdateImagenCanchaCommand : CommandBase<GetImagenCanchaDto>
    {
        public UpdateImagenCanchaCommand(UpdateImagenCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateImagenCanchaDto UpdateDto { get; set; }
    }
}
