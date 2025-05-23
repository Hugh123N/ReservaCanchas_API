using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class CreateReservaCommand : CommandBase<GetReservaDto>
    {
        public CreateReservaCommand(CreateReservaDto createDto) => CreateDto = createDto;
        public CreateReservaDto CreateDto { get; set; }
    }
}
