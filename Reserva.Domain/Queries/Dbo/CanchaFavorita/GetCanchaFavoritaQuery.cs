using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.CanchaFavorita;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class GetCanchaFavoritaQuery : QueryBase<GetCanchaFavoritaDto>
    {
        public GetCanchaFavoritaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
