using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class SearchDiaSemanaQuery : SearchQueryBase<SearchDiaSemanaFilterDto, SearchDiaSemanaDto>
    {
        public SearchDiaSemanaQuery(SearchParamsDto<SearchDiaSemanaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
