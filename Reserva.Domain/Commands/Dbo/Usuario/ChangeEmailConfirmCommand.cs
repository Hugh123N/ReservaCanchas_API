using Reserva.Domain.Commands.Base;
using System;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeEmailConfirmCommand : CommandBase
    {
        public ChangeEmailConfirmCommand(Guid userId, string email, string token)
        {
            UserId = userId;
            Email = email;
            Token = token;
        }

        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
