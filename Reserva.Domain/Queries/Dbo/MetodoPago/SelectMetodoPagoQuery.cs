using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MetodoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.MetodoPago
{
    public class SelectMetodoPagoQuery : SearchQueryBase<SelectMetodoPagoFilterDto, SelectMetodoPagoDto>
    {
        public SelectMetodoPagoQuery(SearchParamsDto<SelectMetodoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
