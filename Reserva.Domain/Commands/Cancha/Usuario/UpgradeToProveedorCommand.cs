using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class UpgradeToProveedorCommand : CommandBase<GetUsuarioDto>
    {
        public UpgradeToProveedorCommand(Guid userId, UpgradeToProveedorDto upgradeDto)
        {
            UserId = userId;
            UpgradeDto = upgradeDto;
        }

        public Guid UserId { get; set; }
        public UpgradeToProveedorDto UpgradeDto { get; set; }
    }
}
