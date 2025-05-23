using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class SearchUsuarioQuery : SearchQueryBase<SearchUsuarioFilterDto, SearchUsuarioDto>
    {
        public SearchUsuarioQuery(SearchParamsDto<SearchUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
