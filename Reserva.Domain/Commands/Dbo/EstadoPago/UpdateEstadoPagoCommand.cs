using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Domain.Commands.Dbo.EstadoPago
{
    public class UpdateEstadoPagoCommand : CommandBase<GetEstadoPagoDto>
    {
        public UpdateEstadoPagoCommand(UpdateEstadoPagoDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoPagoDto UpdateDto { get; set; }
    }
}
