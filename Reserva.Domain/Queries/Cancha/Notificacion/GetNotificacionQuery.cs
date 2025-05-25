using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class GetNotificacionQuery : QueryBase<GetNotificacionDto>
    {
        public GetNotificacionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
