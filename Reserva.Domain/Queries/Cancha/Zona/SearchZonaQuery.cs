using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class SearchZonaQuery : SearchQueryBase<SearchZonaFilterDto, SearchZonaDto>
    {
        public SearchZonaQuery(SearchParamsDto<SearchZonaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
