using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class SearchComprobantePagoPlanQuery : SearchQueryBase<SearchComprobantePagoPlanFilterDto, SearchComprobantePagoPlanDto>
    {
        public SearchComprobantePagoPlanQuery(SearchParamsDto<SearchComprobantePagoPlanFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
