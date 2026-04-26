using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class SearchPagoPlanQuery : SearchQueryBase<SearchPagoPlanFilterDto, SearchPagoPlanDto>
    {
        public SearchPagoPlanQuery(SearchParamsDto<SearchPagoPlanFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
