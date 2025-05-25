using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Notificacion;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class SelectNotificacionQuery : SearchQueryBase<SelectNotificacionFilterDto, SelectNotificacionDto>
    {
        public SelectNotificacionQuery(SearchParamsDto<SelectNotificacionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
