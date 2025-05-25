using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class SearchImagenCanchaQuery : SearchQueryBase<SearchImagenCanchaFilterDto, SearchImagenCanchaDto>
    {
        public SearchImagenCanchaQuery(SearchParamsDto<SearchImagenCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
