using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.CanchaFavorita
{
    public class DeleteCanchaFavoritaCommand : CommandBase
    {
        public DeleteCanchaFavoritaCommand(int id, string idUsuario)
        {
            Id = id;
            IdUsuario = idUsuario;
        }
        public int Id { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
        }
}
