using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class CreateDistritoCommand : CommandBase<GetDistritoDto>
    {
        public CreateDistritoCommand(CreateDistritoDto createDto) => CreateDto = createDto;
        public CreateDistritoDto CreateDto { get; set; }
    }
}
