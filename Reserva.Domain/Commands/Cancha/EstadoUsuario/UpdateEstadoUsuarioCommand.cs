using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class UpdateEstadoUsuarioCommand : CommandBase<GetEstadoUsuarioDto>
    {
        public UpdateEstadoUsuarioCommand(UpdateEstadoUsuarioDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoUsuarioDto UpdateDto { get; set; }
    }
}
