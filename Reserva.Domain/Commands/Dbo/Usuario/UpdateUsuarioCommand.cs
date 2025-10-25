using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Usuario;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class UpdateUsuarioCommand : CommandBase<GetUsuarioDto>
    {
        public UpdateUsuarioCommand(UpdateUsuarioDto updateDto) => UpdateDto = updateDto;
        public UpdateUsuarioDto UpdateDto { get; set; }
    }
}
