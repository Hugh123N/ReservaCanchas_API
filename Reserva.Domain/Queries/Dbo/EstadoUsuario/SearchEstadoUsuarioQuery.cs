using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoUsuario;

namespace Reserva.Domain.Queries.Dbo.EstadoUsuario
{
    public class SearchEstadoUsuarioQuery : SearchQueryBase<SearchEstadoUsuarioFilterDto, SearchEstadoUsuarioDto>
    {
        public SearchEstadoUsuarioQuery(SearchParamsDto<SearchEstadoUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
