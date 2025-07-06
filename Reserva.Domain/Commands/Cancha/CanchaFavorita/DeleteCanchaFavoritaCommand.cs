using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class DeleteCanchaFavoritaCommand : CommandBase
    {
        public DeleteCanchaFavoritaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
