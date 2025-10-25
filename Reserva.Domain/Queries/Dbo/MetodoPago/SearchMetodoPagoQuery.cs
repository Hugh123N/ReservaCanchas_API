using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.MetodoPago;

namespace Reserva.Domain.Queries.Dbo.MetodoPago
{
    public class SearchMetodoPagoQuery : SearchQueryBase<SearchMetodoPagoFilterDto, SearchMetodoPagoDto>
    {
        public SearchMetodoPagoQuery(SearchParamsDto<SearchMetodoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
