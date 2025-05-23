using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class SelectTipoCanchaQuery : SearchQueryBase<SelectTipoCanchaFilterDto, SelectTipoCanchaDto>
    {
        public SelectTipoCanchaQuery(SearchParamsDto<SelectTipoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
