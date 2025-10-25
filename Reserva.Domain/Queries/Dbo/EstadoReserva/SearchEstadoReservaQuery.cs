using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoReserva;

namespace Reserva.Domain.Queries.Dbo.EstadoReserva
{
    public class SearchEstadoReservaQuery : SearchQueryBase<SearchEstadoReservaFilterDto, SearchEstadoReservaDto>
    {
        public SearchEstadoReservaQuery(SearchParamsDto<SearchEstadoReservaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
