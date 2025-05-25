using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class SearchGananciaProveedorQuery : SearchQueryBase<SearchGananciaProveedorFilterDto, SearchGananciaProveedorDto>
    {
        public SearchGananciaProveedorQuery(SearchParamsDto<SearchGananciaProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
