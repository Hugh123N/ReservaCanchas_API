using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class GetCanchaFavoritaQuery : QueryBase<GetCanchaFavoritaDto>
    {
        public GetCanchaFavoritaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
