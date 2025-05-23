using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class SearchReservaQuery : SearchQueryBase<SearchReservaFilterDto, SearchReservaDto>
    {
        public SearchReservaQuery(SearchParamsDto<SearchReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
