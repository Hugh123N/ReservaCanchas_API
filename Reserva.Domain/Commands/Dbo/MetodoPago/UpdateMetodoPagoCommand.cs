using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.MetodoPago;

namespace Reserva.Domain.Commands.Dbo.MetodoPago
{
    public class UpdateMetodoPagoCommand : CommandBase<GetMetodoPagoDto>
    {
        public UpdateMetodoPagoCommand(UpdateMetodoPagoDto updateDto) => UpdateDto = updateDto;
        public UpdateMetodoPagoDto UpdateDto { get; set; }
    }
}
