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
    public class ChangeRoleCommand : CommandBase<LoginResultDto>
    {
        public ChangeRoleCommand(Guid userId, ChangeRoleDto changeRoleDto)
        {
            UserId = userId;
            ChangeRoleDto = changeRoleDto;
        }

        public Guid UserId { get; set; }
        public ChangeRoleDto ChangeRoleDto { get; set; }
    }
}