using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Notificacion;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class SelectNotificacionQuery : SearchQueryBase<SelectNotificacionFilterDto, SelectNotificacionDto>
    {
        public SelectNotificacionQuery(SearchParamsDto<SelectNotificacionFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
