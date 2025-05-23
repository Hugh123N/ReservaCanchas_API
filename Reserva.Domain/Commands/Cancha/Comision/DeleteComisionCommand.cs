using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class DeleteComisionCommand : CommandBase
    {
        public DeleteComisionCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
