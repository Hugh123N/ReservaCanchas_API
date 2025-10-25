using Reserva.Dto.Base;
using Reserva.Dto.Dbo.GananciaProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.GananciaProveedor
{
    public class SelectGananciaProveedorQuery : SearchQueryBase<SelectGananciaProveedorFilterDto, SelectGananciaProveedorDto>
    {
        public SelectGananciaProveedorQuery(SearchParamsDto<SelectGananciaProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
