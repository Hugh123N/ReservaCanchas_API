using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Comision;

namespace Reserva.Domain.Queries.Dbo.Comision
{
    public class SearchComisionQuery : SearchQueryBase<SearchComisionFilterDto, SearchComisionDto>
    {
        public SearchComisionQuery(SearchParamsDto<SearchComisionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
