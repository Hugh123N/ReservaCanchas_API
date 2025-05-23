using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class DeleteDistritoCommand : CommandBase
    {
        public DeleteDistritoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
