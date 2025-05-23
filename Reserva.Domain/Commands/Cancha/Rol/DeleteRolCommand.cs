using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class DeleteRolCommand : CommandBase
    {
        public DeleteRolCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
