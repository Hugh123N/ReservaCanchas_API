using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class CreateRolCommand : CommandBase<GetRolDto>
    {
        public CreateRolCommand(CreateRolDto createDto) => CreateDto = createDto;
        public CreateRolDto CreateDto { get; set; }
    }
}
