using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Base;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class CompletarPagoCommand : CommandBase<GetPagoDto>
    {
        public CompletarPagoDto CompletarDto { get; set; } = null!;
    }
}
