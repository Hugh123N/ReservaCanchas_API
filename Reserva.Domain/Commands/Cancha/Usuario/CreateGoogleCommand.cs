using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateGoogleCommand : CommandBase<LoginResultDto>
    {
        public CreateGoogleCommand(CreateGoogleDto createDto) => CreateDto = createDto;
        public CreateGoogleDto CreateDto { get; set; }
    }
}
