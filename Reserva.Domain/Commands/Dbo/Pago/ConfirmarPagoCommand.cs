using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class ConfirmarPagoCommand : CommandBase<GetPagoDto>
    {
        public ConfirmarPagoCommand(ConfirmarPagoDto confirmarDto) => ConfirmarDto = confirmarDto;
        public ConfirmarPagoDto ConfirmarDto { get; set; }
    }
}
