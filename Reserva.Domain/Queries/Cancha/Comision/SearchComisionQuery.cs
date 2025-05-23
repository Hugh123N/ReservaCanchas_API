using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class SearchComisionQuery : SearchQueryBase<SearchComisionFilterDto, SearchComisionDto>
    {
        public SearchComisionQuery(SearchParamsDto<SearchComisionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
