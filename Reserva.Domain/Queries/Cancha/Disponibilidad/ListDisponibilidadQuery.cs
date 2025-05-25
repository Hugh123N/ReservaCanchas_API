using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class ListDisponibilidadQuery : QueryBase<IEnumerable<ListDisponibilidadDto>>
    {
        public ListDisponibilidadQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
