using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Domain.Queries.Dbo.ImagenCancha
{
    public class SearchImagenCanchaQuery : SearchQueryBase<SearchImagenCanchaFilterDto, SearchImagenCanchaDto>
    {
        public SearchImagenCanchaQuery(SearchParamsDto<SearchImagenCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
