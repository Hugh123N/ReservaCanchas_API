using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class ListCanchaFavoritaQuery : QueryBase<IEnumerable<ListCanchaFavoritaDto>>
    {
        public ListCanchaFavoritaQuery(string idUsuario) => IdUsuario = idUsuario;
        public string IdUsuario { get; set; }
    }
}
