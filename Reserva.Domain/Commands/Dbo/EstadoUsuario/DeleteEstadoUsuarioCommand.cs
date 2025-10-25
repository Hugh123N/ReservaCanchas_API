using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoUsuario
{
    public class DeleteEstadoUsuarioCommand : CommandBase
    {
        public DeleteEstadoUsuarioCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
