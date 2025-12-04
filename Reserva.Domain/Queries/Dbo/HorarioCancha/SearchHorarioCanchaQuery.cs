using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class SearchHorarioCanchaQuery : SearchQueryBase<SearchHorarioCanchaFilterDto, SearchHorarioCanchaDto>
    {
        public SearchHorarioCanchaQuery(SearchParamsDto<SearchHorarioCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
