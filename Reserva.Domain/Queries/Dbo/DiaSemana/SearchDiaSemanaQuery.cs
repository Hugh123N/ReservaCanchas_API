using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DiaSemana;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class SearchDiaSemanaQuery : SearchQueryBase<SearchDiaSemanaFilterDto, SearchDiaSemanaDto>
    {
        public SearchDiaSemanaQuery(SearchParamsDto<SearchDiaSemanaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
