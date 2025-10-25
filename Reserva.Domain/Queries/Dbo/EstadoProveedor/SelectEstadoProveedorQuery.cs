using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoProveedor
{
    public class SelectEstadoProveedorQuery : SearchQueryBase<SelectEstadoProveedorFilterDto, SelectEstadoProveedorDto>
    {
        public SelectEstadoProveedorQuery(SearchParamsDto<SelectEstadoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
