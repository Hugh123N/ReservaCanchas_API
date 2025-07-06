using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class SearchIntentoLoginQuery : SearchQueryBase<SearchIntentoLoginFilterDto, SearchIntentoLoginDto>
    {
        public SearchIntentoLoginQuery(SearchParamsDto<SearchIntentoLoginFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
