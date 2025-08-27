using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class DeleteUsuarioCommand : CommandBase
    {
        public DeleteUsuarioCommand(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
