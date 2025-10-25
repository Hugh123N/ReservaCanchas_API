using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Domain.Queries.Dbo.EstadoPago
{
    public class SearchEstadoPagoQuery : SearchQueryBase<SearchEstadoPagoFilterDto, SearchEstadoPagoDto>
    {
        public SearchEstadoPagoQuery(SearchParamsDto<SearchEstadoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
