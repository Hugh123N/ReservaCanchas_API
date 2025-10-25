using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class UpdateReservaCommand : CommandBase<GetReservaDto>
    {
        public UpdateReservaCommand(UpdateReservaDto updateDto) => UpdateDto = updateDto;
        public UpdateReservaDto UpdateDto { get; set; }
    }
}
