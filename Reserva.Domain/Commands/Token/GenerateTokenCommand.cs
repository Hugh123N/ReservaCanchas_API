using Reserva.Domain.Commands.Base;
using Reserva.Dto.Token;
using Reserva.Entity;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Token
{
    public class GenerateTokenCommand : CommandBase<AccessTokenDto>
    {
        public GenerateTokenCommand(string applicationCode, ApplicationUser user)
        {
            ApplicationCode = applicationCode;
            Usuario = user;
        }

        public string ApplicationCode { get; set; }
        public ApplicationUser Usuario { get; set; }
    }
}
