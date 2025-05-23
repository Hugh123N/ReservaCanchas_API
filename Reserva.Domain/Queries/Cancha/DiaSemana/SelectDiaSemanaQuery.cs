using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class SelectDiaSemanaQuery : SearchQueryBase<SelectDiaSemanaFilterDto, SelectDiaSemanaDto>
    {
        public SelectDiaSemanaQuery(SearchParamsDto<SelectDiaSemanaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
