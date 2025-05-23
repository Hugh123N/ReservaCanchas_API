using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class SearchCanchaQuery : SearchQueryBase<SearchCanchaFilterDto, SearchCanchaDto>
    {
        public SearchCanchaQuery(SearchParamsDto<SearchCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
