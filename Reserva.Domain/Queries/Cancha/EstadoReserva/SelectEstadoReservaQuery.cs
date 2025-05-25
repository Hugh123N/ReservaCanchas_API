using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class SelectEstadoReservaQuery : SearchQueryBase<SelectEstadoReservaFilterDto, SelectEstadoReservaDto>
    {
        public SelectEstadoReservaQuery(SearchParamsDto<SelectEstadoReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
