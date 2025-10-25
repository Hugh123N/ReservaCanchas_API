using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DetallePago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.DetallePago
{
    public class SelectDetallePagoQuery : SearchQueryBase<SelectDetallePagoFilterDto, SelectDetallePagoDto>
    {
        public SelectDetallePagoQuery(SearchParamsDto<SelectDetallePagoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
