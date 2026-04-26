using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class UpdatePlaneCommand : CommandBase<GetPlaneDto>
    {
        public UpdatePlaneCommand(UpdatePlaneDto updateDto) => UpdateDto = updateDto;
        public UpdatePlaneDto UpdateDto { get; set; }
    }
}
