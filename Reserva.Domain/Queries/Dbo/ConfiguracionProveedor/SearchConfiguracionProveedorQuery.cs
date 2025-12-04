using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class SearchConfiguracionProveedorQuery : SearchQueryBase<SearchConfiguracionProveedorFilterDto, SearchConfiguracionProveedorDto>
    {
        public SearchConfiguracionProveedorQuery(SearchParamsDto<SearchConfiguracionProveedorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
