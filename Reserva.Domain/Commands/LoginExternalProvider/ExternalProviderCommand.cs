using Reserva.Dto.Dbo.Usuario;
using Reserva.Domain.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.LoginExternalProvider
{
    public class ExternalProviderCommand : CommandBase<Entity.ApplicationUser>
    {
        public ExternalProviderCommand(CreateAndLoginDto createAndLoginDto) => CreateDto = createAndLoginDto;
        public CreateAndLoginDto CreateDto { get; set; }
    }
}
