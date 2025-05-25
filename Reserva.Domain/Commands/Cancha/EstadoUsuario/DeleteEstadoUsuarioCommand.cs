using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class DeleteEstadoUsuarioCommand : CommandBase
    {
        public DeleteEstadoUsuarioCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
