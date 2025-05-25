using Reserva.Dto.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class SelectMetodoPagoQuery : SearchQueryBase<SelectMetodoPagoFilterDto, SelectMetodoPagoDto>
    {
        public SelectMetodoPagoQuery(SearchParamsDto<SelectMetodoPagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
