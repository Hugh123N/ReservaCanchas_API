using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Token;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Token
{
    public class GenerateTokenCommand : CommandBase<AccessTokenDto>
    {
        public GenerateTokenCommand(string applicationCode, ApplicationUser user, int? idUsuarioNegocio = null)
        {
            ApplicationCode = applicationCode;
            Usuario = user;
            this.idUsuarioNegocio = idUsuarioNegocio;
        }

        public string ApplicationCode { get; set; }
        public ApplicationUser Usuario { get; set; }
        public int? idUsuarioNegocio { get; set; }
    }
}
