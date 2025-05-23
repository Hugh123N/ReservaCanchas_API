using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class UpdateRolCommand : CommandBase<GetRolDto>
    {
        public UpdateRolCommand(UpdateRolDto updateDto) => UpdateDto = updateDto;
        public UpdateRolDto UpdateDto { get; set; }
    }
}
