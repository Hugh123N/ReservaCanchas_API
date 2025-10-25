using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class CreateUsuarioCommand : CommandBase<GetUsuarioDto>
    {
        public CreateUsuarioCommand(CreateUsuarioDto createDto){
            CreateDto = createDto;
        }
        public CreateUsuarioDto CreateDto { get; set; }
    }
}
