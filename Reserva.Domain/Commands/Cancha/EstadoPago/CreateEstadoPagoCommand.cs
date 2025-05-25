using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoPago;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class CreateEstadoPagoCommand : CommandBase<GetEstadoPagoDto>
    {
        public CreateEstadoPagoCommand(CreateEstadoPagoDto createDto) => CreateDto = createDto;
        public CreateEstadoPagoDto CreateDto { get; set; }
    }
}
