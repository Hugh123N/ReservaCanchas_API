using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Domain.Commands.Dbo.EstadoPago
{
    public class CreateEstadoPagoCommand : CommandBase<GetEstadoPagoDto>
    {
        public CreateEstadoPagoCommand(CreateEstadoPagoDto createDto) => CreateDto = createDto;
        public CreateEstadoPagoDto CreateDto { get; set; }
    }
}
