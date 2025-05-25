using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoPago;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class UpdateEstadoPagoCommand : CommandBase<GetEstadoPagoDto>
    {
        public UpdateEstadoPagoCommand(UpdateEstadoPagoDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoPagoDto UpdateDto { get; set; }
    }
}
