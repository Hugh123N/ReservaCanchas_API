using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoPago
{
    public class SelectEstadoPagoQuery : SearchQueryBase<SelectEstadoPagoFilterDto, SelectEstadoPagoDto>
    {
        public SelectEstadoPagoQuery(SearchParamsDto<SelectEstadoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
