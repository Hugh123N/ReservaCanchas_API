using Reserva.Domain.Commands.Base;
using Reserva.Dto.Token;
using Reserva.Entity;
using Reserva.Entity.Models;

namespace Reserva.Domain.Commands.Token
{
    public class GenerateTokenCommand : CommandBase<AccessTokenDto>
    {
        public GenerateTokenCommand(ApplicationUser user, IList<string> roles)
        {
            Usuario = user;
            Roles = roles;
        }

        public ApplicationUser Usuario { get; set; }
        public IList<string> Roles { get; set; }
    }
}
