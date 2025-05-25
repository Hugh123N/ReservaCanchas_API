using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoPago;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class SearchEstadoPagoQuery : SearchQueryBase<SearchEstadoPagoFilterDto, SearchEstadoPagoDto>
    {
        public SearchEstadoPagoQuery(SearchParamsDto<SearchEstadoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
