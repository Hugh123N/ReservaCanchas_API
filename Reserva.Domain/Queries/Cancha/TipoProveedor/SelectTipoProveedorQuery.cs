using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class SelectTipoProveedorQuery : SearchQueryBase<SelectTipoProveedorFilterDto, SelectTipoProveedorDto>
    {
        public SelectTipoProveedorQuery(SearchParamsDto<SelectTipoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
