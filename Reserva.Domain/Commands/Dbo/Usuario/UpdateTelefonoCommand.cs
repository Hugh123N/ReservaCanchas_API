using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class UpdateTelefonoCommand : CommandBase
    {
        public UpdateTelefonoCommand(Guid idUsuario, string telefono)
        {
            IdUsuario = idUsuario;
            Telefono = telefono;
        }

        public Guid IdUsuario { get; set; }
        public string Telefono { get; set; }
    }
}
