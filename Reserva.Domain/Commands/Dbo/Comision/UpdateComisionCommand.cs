using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Comision;

namespace Reserva.Domain.Commands.Dbo.Comision
{
    public class UpdateComisionCommand : CommandBase<GetComisionDto>
    {
        public UpdateComisionCommand(UpdateComisionDto updateDto) => UpdateDto = updateDto;
        public UpdateComisionDto UpdateDto { get; set; }
    }
}
