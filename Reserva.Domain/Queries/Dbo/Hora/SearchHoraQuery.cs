using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class SearchHoraQuery : SearchQueryBase<SearchHoraFilterDto, SearchHoraDto>
    {
        public SearchHoraQuery(SearchParamsDto<SearchHoraFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
