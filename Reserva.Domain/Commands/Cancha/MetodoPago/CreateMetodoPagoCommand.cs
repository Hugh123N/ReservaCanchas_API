using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class CreateMetodoPagoCommand : CommandBase<GetMetodoPagoDto>
    {
        public CreateMetodoPagoCommand(CreateMetodoPagoDto createDto) => CreateDto = createDto;
        public CreateMetodoPagoDto CreateDto { get; set; }
    }
}
