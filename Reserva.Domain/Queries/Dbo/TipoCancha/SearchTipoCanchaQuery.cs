using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoCancha;

namespace Reserva.Domain.Queries.Dbo.TipoCancha
{
    public class SearchTipoCanchaQuery : SearchQueryBase<SearchTipoCanchaFilterDto, SearchTipoCanchaDto>
    {
        public SearchTipoCanchaQuery(SearchParamsDto<SearchTipoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
