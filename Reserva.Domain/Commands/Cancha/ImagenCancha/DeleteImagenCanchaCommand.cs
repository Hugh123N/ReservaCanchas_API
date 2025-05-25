using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class DeleteImagenCanchaCommand : CommandBase
    {
        public DeleteImagenCanchaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
