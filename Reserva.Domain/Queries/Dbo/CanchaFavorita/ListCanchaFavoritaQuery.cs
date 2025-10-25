using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class ListCanchaFavoritaQuery : QueryBase<IEnumerable<ListCanchaFavoritaDto>>
    {
        public ListCanchaFavoritaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
