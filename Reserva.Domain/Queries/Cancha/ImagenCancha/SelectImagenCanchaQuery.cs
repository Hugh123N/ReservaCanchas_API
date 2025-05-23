using Reserva.Dto.Base;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class SelectImagenCanchaQuery : SearchQueryBase<SelectImagenCanchaFilterDto, SelectImagenCanchaDto>
    {
        public SelectImagenCanchaQuery(SearchParamsDto<SelectImagenCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
