using Reserva.Dto.Base;
using Reserva.Dto.Cancha.IntentoLogin;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class SelectIntentoLoginQuery : SearchQueryBase<SelectIntentoLoginFilterDto, SelectIntentoLoginDto>
    {
        public SelectIntentoLoginQuery(SearchParamsDto<SelectIntentoLoginFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
