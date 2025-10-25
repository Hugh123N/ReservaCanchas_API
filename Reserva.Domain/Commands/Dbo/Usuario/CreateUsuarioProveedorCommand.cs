using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class CreateUsuarioProveedorCommand : CommandBase<GetUsuarioDto>
    {
        public CreateUsuarioProveedorCommand(CreateUsuarioProveedorDto createDto)
        {
            CreateDto = createDto;
        }
        public CreateUsuarioProveedorDto CreateDto { get; set; }
    }
}
