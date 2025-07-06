using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class ListCanchaFavoritaQuery : QueryBase<IEnumerable<ListCanchaFavoritaDto>>
    {
        public ListCanchaFavoritaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
