using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.IntentoLogin;

namespace Reserva.Domain.Queries.Dbo.IntentoLogin
{
    public class SearchIntentoLoginQuery : SearchQueryBase<SearchIntentoLoginFilterDto, SearchIntentoLoginDto>
    {
        public SearchIntentoLoginQuery(SearchParamsDto<SearchIntentoLoginFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
