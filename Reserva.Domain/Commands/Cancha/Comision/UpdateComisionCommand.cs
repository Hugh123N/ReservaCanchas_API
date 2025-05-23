using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class UpdateComisionCommand : CommandBase<GetComisionDto>
    {
        public UpdateComisionCommand(UpdateComisionDto updateDto) => UpdateDto = updateDto;
        public UpdateComisionDto UpdateDto { get; set; }
    }
}
