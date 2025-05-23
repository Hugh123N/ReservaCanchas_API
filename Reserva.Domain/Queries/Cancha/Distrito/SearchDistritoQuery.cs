using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class SearchDistritoQuery : SearchQueryBase<SearchDistritoFilterDto, SearchDistritoDto>
    {
        public SearchDistritoQuery(SearchParamsDto<SearchDistritoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
