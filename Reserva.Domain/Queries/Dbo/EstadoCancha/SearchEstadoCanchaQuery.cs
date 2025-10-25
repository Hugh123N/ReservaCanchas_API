using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoCancha;

namespace Reserva.Domain.Queries.Dbo.EstadoCancha
{
    public class SearchEstadoCanchaQuery : SearchQueryBase<SearchEstadoCanchaFilterDto, SearchEstadoCanchaDto>
    {
        public SearchEstadoCanchaQuery(SearchParamsDto<SearchEstadoCanchaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
