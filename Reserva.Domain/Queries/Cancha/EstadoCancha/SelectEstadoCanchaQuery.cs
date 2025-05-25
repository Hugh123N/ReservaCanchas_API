using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class SelectEstadoCanchaQuery : SearchQueryBase<SelectEstadoCanchaFilterDto, SelectEstadoCanchaDto>
    {
        public SelectEstadoCanchaQuery(SearchParamsDto<SelectEstadoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
