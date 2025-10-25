using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class SelectCanchaQuery : SearchQueryBase<SelectCanchaFilterDto, SelectCanchaDto>
    {
        public SelectCanchaQuery(SearchParamsDto<SelectCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
