using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class CreateComisionCommand : CommandBase<GetComisionDto>
    {
        public CreateComisionCommand(CreateComisionDto createDto) => CreateDto = createDto;
        public CreateComisionDto CreateDto { get; set; }
    }
}
