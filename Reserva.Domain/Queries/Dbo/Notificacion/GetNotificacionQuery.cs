using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class GetNotificacionQuery : QueryBase<GetNotificacionDto>
    {
        public GetNotificacionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
