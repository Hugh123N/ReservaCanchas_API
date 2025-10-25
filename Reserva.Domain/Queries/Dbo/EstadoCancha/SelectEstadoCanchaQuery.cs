using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoCancha
{
    public class SelectEstadoCanchaQuery : SearchQueryBase<SelectEstadoCanchaFilterDto, SelectEstadoCanchaDto>
    {
        public SelectEstadoCanchaQuery(SearchParamsDto<SelectEstadoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
