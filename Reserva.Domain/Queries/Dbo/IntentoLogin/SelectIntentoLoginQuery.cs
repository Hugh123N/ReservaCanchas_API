using Reserva.Dto.Base;
using Reserva.Dto.Dbo.IntentoLogin;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.IntentoLogin
{
    public class SelectIntentoLoginQuery : SearchQueryBase<SelectIntentoLoginFilterDto, SelectIntentoLoginDto>
    {
        public SelectIntentoLoginQuery(SearchParamsDto<SelectIntentoLoginFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
