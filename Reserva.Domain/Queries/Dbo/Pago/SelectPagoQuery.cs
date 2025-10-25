using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Pago
{
    public class SelectPagoQuery : SearchQueryBase<SelectPagoFilterDto, SelectPagoDto>
    {
        public SelectPagoQuery(SearchParamsDto<SelectPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
