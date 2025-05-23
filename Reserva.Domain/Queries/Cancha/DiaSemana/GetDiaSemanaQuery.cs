using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class GetDiaSemanaQuery : QueryBase<GetDiaSemanaDto>
    {
        public GetDiaSemanaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
