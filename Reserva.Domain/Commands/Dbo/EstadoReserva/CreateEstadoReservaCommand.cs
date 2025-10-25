using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoReserva;

namespace Reserva.Domain.Commands.Dbo.EstadoReserva
{
    public class CreateEstadoReservaCommand : CommandBase<GetEstadoReservaDto>
    {
        public CreateEstadoReservaCommand(CreateEstadoReservaDto createDto) => CreateDto = createDto;
        public CreateEstadoReservaDto CreateDto { get; set; }
    }
}
