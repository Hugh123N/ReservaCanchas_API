using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class SelectDiaSemanaQuery : SearchQueryBase<SelectDiaSemanaFilterDto, SelectDiaSemanaDto>
    {
        public SelectDiaSemanaQuery(SearchParamsDto<SelectDiaSemanaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
