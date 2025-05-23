using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class SearchProvinciaQuery : SearchQueryBase<SearchProvinciaFilterDto, SearchProvinciaDto>
    {
        public SearchProvinciaQuery(SearchParamsDto<SearchProvinciaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
