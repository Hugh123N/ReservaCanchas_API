using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class SelectDistritoQuery : SearchQueryBase<SelectDistritoFilterDto, SelectDistritoDto>
    {
        public SelectDistritoQuery(SearchParamsDto<SelectDistritoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
