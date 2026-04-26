using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class CreatePlaneCommand : CommandBase<GetPlaneDto>
    {
        public CreatePlaneCommand(CreatePlaneDto createDto) => CreateDto = createDto;
        public CreatePlaneDto CreateDto { get; set; }
    }
}
