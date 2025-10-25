using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoUsuario;

namespace Reserva.Domain.Commands.Dbo.EstadoUsuario
{
    public class CreateEstadoUsuarioCommand : CommandBase<GetEstadoUsuarioDto>
    {
        public CreateEstadoUsuarioCommand(CreateEstadoUsuarioDto createDto) => CreateDto = createDto;
        public CreateEstadoUsuarioDto CreateDto { get; set; }
    }
}
