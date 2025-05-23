using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class GetDisponibilidadQuery : QueryBase<GetDisponibilidadDto>
    {
        public GetDisponibilidadQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
