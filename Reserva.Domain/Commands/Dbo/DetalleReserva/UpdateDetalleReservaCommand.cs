using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class UpdateDetalleReservaCommand : CommandBase<GetDetalleReservaDto>
    {
        public UpdateDetalleReservaCommand(UpdateDetalleReservaDto updateDto) => UpdateDto = updateDto;
        public UpdateDetalleReservaDto UpdateDto { get; set; }
    }
}
