using Reserva.Dto.Dbo.Disponibilidad;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Disponibilidad
{
    public class ListDisponibilidadQuery : QueryBase<IEnumerable<ListDisponibilidadDto>>
    {
        public ListDisponibilidadQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
