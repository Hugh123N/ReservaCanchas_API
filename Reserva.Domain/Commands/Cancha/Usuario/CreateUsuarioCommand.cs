using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateUsuarioCommand : CommandBase<GetUsuarioDto>
    {
        public CreateUsuarioCommand(CreateUsuarioDto createDto) => CreateDto = createDto;
        public CreateUsuarioDto CreateDto { get; set; }
    }
}
