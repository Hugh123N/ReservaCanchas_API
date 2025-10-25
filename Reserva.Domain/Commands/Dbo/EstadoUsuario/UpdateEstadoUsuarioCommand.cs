using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoUsuario;

namespace Reserva.Domain.Commands.Dbo.EstadoUsuario
{
    public class UpdateEstadoUsuarioCommand : CommandBase<GetEstadoUsuarioDto>
    {
        public UpdateEstadoUsuarioCommand(UpdateEstadoUsuarioDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoUsuarioDto UpdateDto { get; set; }
    }
}
