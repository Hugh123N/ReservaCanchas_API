using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class SearchReservaQuery : SearchQueryBase<SearchReservaFilterDto, SearchReservaDto>
    {
        public SearchReservaQuery(SearchParamsDto<SearchReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
