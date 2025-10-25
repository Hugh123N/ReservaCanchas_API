using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Proveedor
{
    public class SelectProveedorQuery : SearchQueryBase<SelectProveedorFilterDto, SelectProveedorDto>
    {
        public SelectProveedorQuery(SearchParamsDto<SelectProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
