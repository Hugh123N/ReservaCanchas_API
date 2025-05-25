using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class SelectEstadoProveedorQuery : SearchQueryBase<SelectEstadoProveedorFilterDto, SelectEstadoProveedorDto>
    {
        public SelectEstadoProveedorQuery(SearchParamsDto<SelectEstadoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
