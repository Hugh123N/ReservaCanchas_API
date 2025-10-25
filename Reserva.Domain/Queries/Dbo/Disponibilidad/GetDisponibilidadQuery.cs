using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Disponibilidad;

namespace Reserva.Domain.Queries.Dbo.Disponibilidad
{
    public class GetDisponibilidadQuery : QueryBase<GetDisponibilidadDto>
    {
        public GetDisponibilidadQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
