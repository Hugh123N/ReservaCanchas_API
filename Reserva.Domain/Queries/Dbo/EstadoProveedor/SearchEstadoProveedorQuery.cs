using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoProveedor;

namespace Reserva.Domain.Queries.Dbo.EstadoProveedor
{
    public class SearchEstadoProveedorQuery : SearchQueryBase<SearchEstadoProveedorFilterDto, SearchEstadoProveedorDto>
    {
        public SearchEstadoProveedorQuery(SearchParamsDto<SearchEstadoProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
