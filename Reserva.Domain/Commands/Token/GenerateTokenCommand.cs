using Reserva.Domain.Commands.Base;
using Reserva.Dto.Token;
using Reserva.Entity;
using Reserva.Entity.Models;

namespace Reserva.Domain.Commands.Token
{
    public class GenerateTokenCommand : CommandBase<AccessTokenDto>
    {
        public GenerateTokenCommand(ApplicationUser user)
        {
            Usuario = user;
        }

        public ApplicationUser Usuario { get; set; }
    }
}
