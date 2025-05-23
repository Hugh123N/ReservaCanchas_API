using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class SearchEstadoProveedorQuery : SearchQueryBase<SearchEstadoProveedorFilterDto, SearchEstadoProveedorDto>
    {
        public SearchEstadoProveedorQuery(SearchParamsDto<SearchEstadoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
