using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class SelectReservaQuery : SearchQueryBase<SelectReservaFilterDto, SelectReservaDto>
    {
        public SelectReservaQuery(SearchParamsDto<SelectReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
