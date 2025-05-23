using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Domain.Commands.Cancha.Pago
{
    public class CreatePagoCommand : CommandBase<GetPagoDto>
    {
        public CreatePagoCommand(CreatePagoDto createDto) => CreateDto = createDto;
        public CreatePagoDto CreateDto { get; set; }
    }
}
