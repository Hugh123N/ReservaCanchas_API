using Reserva.Domain.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class ForgotPasswordCommand : CommandBase
    {
        public ForgotPasswordCommand(string email, string host)
        {
            Email = email;
            Host = host;
        }

        public string Email { get; set; }
        public string Host { get; set; }
    }
}
