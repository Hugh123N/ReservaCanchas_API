using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class SearchCanchaFavoritaQuery : SearchQueryBase<SearchCanchaFavoritaFilterDto, SearchCanchaFavoritaDto>
    {
        public SearchCanchaFavoritaQuery(SearchParamsDto<SearchCanchaFavoritaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
