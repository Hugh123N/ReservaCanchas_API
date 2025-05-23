using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class SearchEstadoReservaQuery : SearchQueryBase<SearchEstadoReservaFilterDto, SearchEstadoReservaDto>
    {
        public SearchEstadoReservaQuery(SearchParamsDto<SearchEstadoReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
