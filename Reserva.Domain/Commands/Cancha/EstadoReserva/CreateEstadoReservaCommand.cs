using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class CreateEstadoReservaCommand : CommandBase<GetEstadoReservaDto>
    {
        public CreateEstadoReservaCommand(CreateEstadoReservaDto createDto) => CreateDto = createDto;
        public CreateEstadoReservaDto CreateDto { get; set; }
    }
}
