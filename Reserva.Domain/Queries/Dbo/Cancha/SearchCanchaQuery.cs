using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class SearchCanchaQuery : SearchQueryBase<SearchCanchaFilterDto, SearchCanchaDto>
    {
        public SearchCanchaQuery(SearchParamsDto<SearchCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
