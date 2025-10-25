using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoReserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoReserva
{
    public class SelectEstadoReservaQuery : SearchQueryBase<SelectEstadoReservaFilterDto, SelectEstadoReservaDto>
    {
        public SelectEstadoReservaQuery(SearchParamsDto<SelectEstadoReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
