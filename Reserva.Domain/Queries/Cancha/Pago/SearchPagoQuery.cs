using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class SearchPagoQuery : SearchQueryBase<SearchPagoFilterDto, SearchPagoDto>
    {
        public SearchPagoQuery(SearchParamsDto<SearchPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
