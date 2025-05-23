using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class SelectZonaQuery : SearchQueryBase<SelectZonaFilterDto, SelectZonaDto>
    {
        public SelectZonaQuery(SearchParamsDto<SelectZonaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
