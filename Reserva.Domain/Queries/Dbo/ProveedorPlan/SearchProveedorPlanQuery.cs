using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class SearchProveedorPlanQuery : SearchQueryBase<SearchProveedorPlanFilterDto, SearchProveedorPlanDto>
    {
        public SearchProveedorPlanQuery(SearchParamsDto<SearchProveedorPlanFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
