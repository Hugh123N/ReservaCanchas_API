using Reserva.Dto.Base;
using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class SelectGananciaProveedorQuery : SearchQueryBase<SelectGananciaProveedorFilterDto, SelectGananciaProveedorDto>
    {
        public SelectGananciaProveedorQuery(SearchParamsDto<SelectGananciaProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
