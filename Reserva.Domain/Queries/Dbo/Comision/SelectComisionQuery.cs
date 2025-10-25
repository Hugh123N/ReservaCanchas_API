using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Comision;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Comision
{
    public class SelectComisionQuery : SearchQueryBase<SelectComisionFilterDto, SelectComisionDto>
    {
        public SelectComisionQuery(SearchParamsDto<SelectComisionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
