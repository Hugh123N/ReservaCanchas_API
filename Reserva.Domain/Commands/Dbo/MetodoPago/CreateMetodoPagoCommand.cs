using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.MetodoPago;

namespace Reserva.Domain.Commands.Dbo.MetodoPago
{
    public class CreateMetodoPagoCommand : CommandBase<GetMetodoPagoDto>
    {
        public CreateMetodoPagoCommand(CreateMetodoPagoDto createDto) => CreateDto = createDto;
        public CreateMetodoPagoDto CreateDto { get; set; }
    }
}
