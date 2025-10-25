using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoProveedor;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class SearchTipoProveedorQuery : SearchQueryBase<SearchTipoProveedorFilterDto, SearchTipoProveedorDto>
    {
        public SearchTipoProveedorQuery(SearchParamsDto<SearchTipoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
