using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class UpdateDistritoCommand : CommandBase<GetDistritoDto>
    {
        public UpdateDistritoCommand(UpdateDistritoDto updateDto) => UpdateDto = updateDto;
        public UpdateDistritoDto UpdateDto { get; set; }
    }
}
