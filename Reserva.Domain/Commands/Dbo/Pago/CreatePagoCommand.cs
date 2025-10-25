using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class CreatePagoCommand : CommandBase<GetPagoDto>
    {
        public CreatePagoCommand(CreatePagoDto createDto) => CreateDto = createDto;
        public CreatePagoDto CreateDto { get; set; }
    }
}
