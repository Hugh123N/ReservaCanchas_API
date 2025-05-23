using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class SearchMetodoPagoQuery : SearchQueryBase<SearchMetodoPagoFilterDto, SearchMetodoPagoDto>
    {
        public SearchMetodoPagoQuery(SearchParamsDto<SearchMetodoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
