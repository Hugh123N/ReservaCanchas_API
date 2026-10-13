using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeEmailRequestCommand : CommandBase
    {
        public ChangeEmailRequestCommand(string newEmail, string host)
        {
            NewEmail = newEmail;
            Host = host;
        }

        public string NewEmail { get; set; }
        public string Host { get; set; }
    }
}
