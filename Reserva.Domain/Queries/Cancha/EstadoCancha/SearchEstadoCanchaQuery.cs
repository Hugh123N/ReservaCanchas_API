using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class SearchEstadoCanchaQuery : SearchQueryBase<SearchEstadoCanchaFilterDto, SearchEstadoCanchaDto>
    {
        public SearchEstadoCanchaQuery(SearchParamsDto<SearchEstadoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
