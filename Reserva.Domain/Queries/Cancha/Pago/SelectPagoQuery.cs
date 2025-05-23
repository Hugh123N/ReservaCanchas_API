using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Pago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class SelectPagoQuery : SearchQueryBase<SelectPagoFilterDto, SelectPagoDto>
    {
        public SelectPagoQuery(SearchParamsDto<SelectPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
