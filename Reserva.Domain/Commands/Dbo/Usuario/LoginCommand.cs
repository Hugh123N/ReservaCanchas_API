using Reserva.Domain.Commands.Base;
using Reserva.Dto.User;

namespace Reserva.Domain.Commands.User
{
    public class LoginCommand : CommandBase<LoginResultDto>
    {
        public LoginCommand(LoginDto loginDto) => LoginDto = loginDto;
        public LoginDto LoginDto { get; set; }
    }
}
