using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class SearchTipoCanchaQuery : SearchQueryBase<SearchTipoCanchaFilterDto, SearchTipoCanchaDto>
    {
        public SearchTipoCanchaQuery(SearchParamsDto<SearchTipoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
