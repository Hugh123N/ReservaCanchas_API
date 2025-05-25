using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class UpdateMetodoPagoCommand : CommandBase<GetMetodoPagoDto>
    {
        public UpdateMetodoPagoCommand(UpdateMetodoPagoDto updateDto) => UpdateDto = updateDto;
        public UpdateMetodoPagoDto UpdateDto { get; set; }
    }
}
