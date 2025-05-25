using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class DeleteUsuarioCommand : CommandBase
    {
        public DeleteUsuarioCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
