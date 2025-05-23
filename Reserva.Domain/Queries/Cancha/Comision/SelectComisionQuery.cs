using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class SelectComisionQuery : SearchQueryBase<SelectComisionFilterDto, SelectComisionDto>
    {
        public SelectComisionQuery(SearchParamsDto<SelectComisionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
