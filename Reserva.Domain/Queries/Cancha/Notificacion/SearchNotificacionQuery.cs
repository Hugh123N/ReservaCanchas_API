using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class SearchNotificacionQuery : SearchQueryBase<SearchNotificacionFilterDto, SearchNotificacionDto>
    {
        public SearchNotificacionQuery(SearchParamsDto<SearchNotificacionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
