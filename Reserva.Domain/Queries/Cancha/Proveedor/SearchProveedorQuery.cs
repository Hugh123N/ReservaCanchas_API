using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class SearchProveedorQuery : SearchQueryBase<SearchProveedorFilterDto, SearchProveedorDto>
    {
        public SearchProveedorQuery(SearchParamsDto<SearchProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
