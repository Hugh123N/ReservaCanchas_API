using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class SearchEstadoUsuarioQuery : SearchQueryBase<SearchEstadoUsuarioFilterDto, SearchEstadoUsuarioDto>
    {
        public SearchEstadoUsuarioQuery(SearchParamsDto<SearchEstadoUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
