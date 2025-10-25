using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class CreateReservaCommand : CommandBase<GetReservaDto>
    {
        public CreateReservaCommand(CreateReservaDto createDto) => CreateDto = createDto;
        public CreateReservaDto CreateDto { get; set; }
    }
}
