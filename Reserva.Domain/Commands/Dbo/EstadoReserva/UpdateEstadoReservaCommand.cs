using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoReserva;

namespace Reserva.Domain.Commands.Dbo.EstadoReserva
{
    public class UpdateEstadoReservaCommand : CommandBase<GetEstadoReservaDto>
    {
        public UpdateEstadoReservaCommand(UpdateEstadoReservaDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoReservaDto UpdateDto { get; set; }
    }
}
