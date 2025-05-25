using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Domain.Commands.Cancha.Pago
{
    public class UpdatePagoCommand : CommandBase<GetPagoDto>
    {
        public UpdatePagoCommand(UpdatePagoDto updateDto) => UpdateDto = updateDto;
        public UpdatePagoDto UpdateDto { get; set; }
    }
}
