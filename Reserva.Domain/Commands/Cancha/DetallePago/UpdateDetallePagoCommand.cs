using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DetallePago;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class UpdateDetallePagoCommand : CommandBase<GetDetallePagoDto>
    {
        public UpdateDetallePagoCommand(UpdateDetallePagoDto updateDto) => UpdateDto = updateDto;
        public UpdateDetallePagoDto UpdateDto { get; set; }
    }
}
