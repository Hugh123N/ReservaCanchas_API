using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.GananciaProveedor;

namespace Reserva.Domain.Queries.Dbo.GananciaProveedor
{
    public class SearchGananciaProveedorQuery : SearchQueryBase<SearchGananciaProveedorFilterDto, SearchGananciaProveedorDto>
    {
        public SearchGananciaProveedorQuery(SearchParamsDto<SearchGananciaProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
