using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Domain.Commands.Dbo.DetallePago
{
    public class UpdateDetallePagoCommand : CommandBase<GetDetallePagoDto>
    {
        public UpdateDetallePagoCommand(UpdateDetallePagoDto updateDto) => UpdateDto = updateDto;
        public UpdateDetallePagoDto UpdateDto { get; set; }
    }
}
