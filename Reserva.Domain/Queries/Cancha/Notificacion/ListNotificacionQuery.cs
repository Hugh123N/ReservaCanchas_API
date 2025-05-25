using Reserva.Dto.Cancha.Notificacion;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class ListNotificacionQuery : QueryBase<IEnumerable<ListNotificacionDto>>
    {
        public ListNotificacionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
