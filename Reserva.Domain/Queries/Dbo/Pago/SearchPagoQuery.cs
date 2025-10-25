using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Queries.Dbo.Pago
{
    public class SearchPagoQuery : SearchQueryBase<SearchPagoFilterDto, SearchPagoDto>
    {
        public SearchPagoQuery(SearchParamsDto<SearchPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
