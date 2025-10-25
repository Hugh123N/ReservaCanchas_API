using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class SelectTipoProveedorQuery : SearchQueryBase<SelectTipoProveedorFilterDto, SelectTipoProveedorDto>
    {
        public SelectTipoProveedorQuery(SearchParamsDto<SelectTipoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
