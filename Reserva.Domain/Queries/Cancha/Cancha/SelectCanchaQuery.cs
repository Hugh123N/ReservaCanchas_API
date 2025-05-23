using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class SelectCanchaQuery : SearchQueryBase<SelectCanchaFilterDto, SelectCanchaDto>
    {
        public SelectCanchaQuery(SearchParamsDto<SelectCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
