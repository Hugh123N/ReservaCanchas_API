using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class UpdateReservaCommand : CommandBase<GetReservaDto>
    {
        public UpdateReservaCommand(UpdateReservaDto updateDto) => UpdateDto = updateDto;
        public UpdateReservaDto UpdateDto { get; set; }
    }
}
