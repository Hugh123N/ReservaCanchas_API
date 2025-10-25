using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class CreateAndLoginCommand : CommandBase<LoginResultDto>
    {
        public CreateAndLoginCommand(CreateAndLoginDto createDto) => CreateDto = createDto;
        public CreateAndLoginDto CreateDto { get; set; }
    }
}
