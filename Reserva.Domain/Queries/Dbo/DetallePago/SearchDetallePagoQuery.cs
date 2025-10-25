using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Domain.Queries.Dbo.DetallePago
{
    public class SearchDetallePagoQuery : SearchQueryBase<SearchDetallePagoFilterDto, SearchDetallePagoDto>
    {
        public SearchDetallePagoQuery(SearchParamsDto<SearchDetallePagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
