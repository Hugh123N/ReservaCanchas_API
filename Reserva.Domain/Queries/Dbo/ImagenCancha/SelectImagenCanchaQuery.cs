using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.ImagenCancha
{
    public class SelectImagenCanchaQuery : SearchQueryBase<SelectImagenCanchaFilterDto, SelectImagenCanchaDto>
    {
        public SelectImagenCanchaQuery(SearchParamsDto<SelectImagenCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
