using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class DeleteCanchaCommand : CommandBase
    {
        public DeleteCanchaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
