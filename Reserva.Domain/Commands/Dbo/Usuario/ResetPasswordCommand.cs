using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ResetPasswordCommand : CommandBase
    {
        public ResetPasswordCommand(ResetPasswordDto resetPasswordDto)
        {
            ResetPasswordDto = resetPasswordDto;
        }
        public ResetPasswordDto ResetPasswordDto { get; set; }
    }
}
