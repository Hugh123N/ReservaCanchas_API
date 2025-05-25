using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DetallePago;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class SearchDetallePagoQuery : SearchQueryBase<SearchDetallePagoFilterDto, SearchDetallePagoDto>
    {
        public SearchDetallePagoQuery(SearchParamsDto<SearchDetallePagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
