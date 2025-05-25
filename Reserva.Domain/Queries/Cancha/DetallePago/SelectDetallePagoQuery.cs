using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class SelectDetallePagoQuery : SearchQueryBase<SelectDetallePagoFilterDto, SelectDetallePagoDto>
    {
        public SelectDetallePagoQuery(SearchParamsDto<SelectDetallePagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
