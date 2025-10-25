using Reserva.Dto.Dbo.Notificacion;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class ListNotificacionQuery : QueryBase<IEnumerable<ListNotificacionDto>>
    {
        public ListNotificacionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
