using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DetallePago;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class CreateDetallePagoCommand : CommandBase<GetDetallePagoDto>
    {
        public CreateDetallePagoCommand(CreateDetallePagoDto createDto) => CreateDto = createDto;
        public CreateDetallePagoDto CreateDto { get; set; }
    }
}
