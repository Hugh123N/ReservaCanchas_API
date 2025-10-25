using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Usuario;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class SearchUsuarioQuery : SearchQueryBase<SearchUsuarioFilterDto, SearchUsuarioDto>
    {
        public SearchUsuarioQuery(SearchParamsDto<SearchUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
