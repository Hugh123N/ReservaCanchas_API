using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class CreateDetalleReservaCommand : CommandBase<GetDetalleReservaDto>
    {
        public CreateDetalleReservaCommand(CreateDetalleReservaDto createDto) => CreateDto = createDto;
        public CreateDetalleReservaDto CreateDto { get; set; }
    }
}
