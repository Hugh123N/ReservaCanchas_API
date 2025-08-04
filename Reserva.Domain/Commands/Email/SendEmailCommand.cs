using Reserva.Domain.Commands.Base;
using Reserva.Dto.Email;

namespace Reserva.Domain.Commands.Email
{
    public class SendEmailCommand : CommandBase
    {
        public SendEmailCommand(SendEmailDto emailDto) => EmailDto = emailDto;
        public SendEmailDto EmailDto { get; set; }
    }
}
