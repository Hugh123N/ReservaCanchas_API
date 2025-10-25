using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Domain.Commands.Dbo.DetallePago
{
    public class CreateDetallePagoCommand : CommandBase<GetDetallePagoDto>
    {
        public CreateDetallePagoCommand(CreateDetallePagoDto createDto) => CreateDto = createDto;
        public CreateDetallePagoDto CreateDto { get; set; }
    }
}
