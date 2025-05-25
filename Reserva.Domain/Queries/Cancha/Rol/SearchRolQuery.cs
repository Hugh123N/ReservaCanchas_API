using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class SearchRolQuery : SearchQueryBase<SearchRolFilterDto, SearchRolDto>
    {
        public SearchRolQuery(SearchParamsDto<SearchRolFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
