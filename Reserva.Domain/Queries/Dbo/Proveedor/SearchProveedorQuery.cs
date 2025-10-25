using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Proveedor;

namespace Reserva.Domain.Queries.Dbo.Proveedor
{
    public class SearchProveedorQuery : SearchQueryBase<SearchProveedorFilterDto, SearchProveedorDto>
    {
        public SearchProveedorQuery(SearchParamsDto<SearchProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
