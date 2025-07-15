using Reserva.Domain.Commands.Base;
using Reserva.Dto.Token;
using Reserva.Entity;
using Reserva.Entity.Models;

namespace Reserva.Domain.Commands.Token
{
    public class GenerateTokenCommand : CommandBase<AccessTokenDto>
    {
        public GenerateTokenCommand(Usuario user)
        {
            Usuario = user;
        }

        public Usuario Usuario { get; set; }
    }
}
