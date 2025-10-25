using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class SelectReservaQuery : SearchQueryBase<SelectReservaFilterDto, SelectReservaDto>
    {
        public SelectReservaQuery(SearchParamsDto<SelectReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
