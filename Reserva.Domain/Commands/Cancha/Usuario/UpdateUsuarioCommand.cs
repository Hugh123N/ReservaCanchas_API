using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class UpdateUsuarioCommand : CommandBase<GetUsuarioDto>
    {
        public UpdateUsuarioCommand(UpdateUsuarioDto updateDto) => UpdateDto = updateDto;
        public UpdateUsuarioDto UpdateDto { get; set; }
    }
}
