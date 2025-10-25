using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.TipoCancha
{
    public class SelectTipoCanchaQuery : SearchQueryBase<SelectTipoCanchaFilterDto, SelectTipoCanchaDto>
    {
        public SelectTipoCanchaQuery(SearchParamsDto<SelectTipoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
