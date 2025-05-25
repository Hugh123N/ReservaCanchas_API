using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class CreateEstadoUsuarioCommand : CommandBase<GetEstadoUsuarioDto>
    {
        public CreateEstadoUsuarioCommand(CreateEstadoUsuarioDto createDto) => CreateDto = createDto;
        public CreateEstadoUsuarioDto CreateDto { get; set; }
    }
}
