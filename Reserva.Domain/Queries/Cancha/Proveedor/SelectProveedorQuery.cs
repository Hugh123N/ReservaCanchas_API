using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class SelectProveedorQuery : SearchQueryBase<SelectProveedorFilterDto, SelectProveedorDto>
    {
        public SelectProveedorQuery(SearchParamsDto<SelectProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
