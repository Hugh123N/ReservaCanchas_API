using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class UpdatePagoCommand : CommandBase<GetPagoDto>
    {
        public UpdatePagoCommand(UpdatePagoDto updateDto) => UpdateDto = updateDto;
        public UpdatePagoDto UpdateDto { get; set; }
    }
}
