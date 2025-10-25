using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Comision;

namespace Reserva.Domain.Commands.Dbo.Comision
{
    public class CreateComisionCommand : CommandBase<GetComisionDto>
    {
        public CreateComisionCommand(CreateComisionDto createDto) => CreateDto = createDto;
        public CreateComisionDto CreateDto { get; set; }
    }
}
