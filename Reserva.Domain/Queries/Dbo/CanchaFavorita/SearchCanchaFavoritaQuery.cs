using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.CanchaFavorita;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class SearchCanchaFavoritaQuery : SearchQueryBase<SearchCanchaFavoritaFilterDto, SearchCanchaFavoritaDto>
    {
        public SearchCanchaFavoritaQuery(SearchParamsDto<SearchCanchaFavoritaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
