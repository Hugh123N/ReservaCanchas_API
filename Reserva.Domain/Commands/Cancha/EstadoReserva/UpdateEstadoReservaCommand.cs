using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class UpdateEstadoReservaCommand : CommandBase<GetEstadoReservaDto>
    {
        public UpdateEstadoReservaCommand(UpdateEstadoReservaDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoReservaDto UpdateDto { get; set; }
    }
}
