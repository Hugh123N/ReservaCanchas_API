using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class SearchTipoProveedorQuery : SearchQueryBase<SearchTipoProveedorFilterDto, SearchTipoProveedorDto>
    {
        public SearchTipoProveedorQuery(SearchParamsDto<SearchTipoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
