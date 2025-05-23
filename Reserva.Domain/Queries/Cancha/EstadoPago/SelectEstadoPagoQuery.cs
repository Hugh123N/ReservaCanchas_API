using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class SelectEstadoPagoQuery : SearchQueryBase<SelectEstadoPagoFilterDto, SelectEstadoPagoDto>
    {
        public SelectEstadoPagoQuery(SearchParamsDto<SelectEstadoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
