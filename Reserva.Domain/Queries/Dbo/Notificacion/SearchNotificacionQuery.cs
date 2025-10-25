using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class SearchNotificacionQuery : SearchQueryBase<SearchNotificacionFilterDto, SearchNotificacionDto>
    {
        public SearchNotificacionQuery(SearchParamsDto<SearchNotificacionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
