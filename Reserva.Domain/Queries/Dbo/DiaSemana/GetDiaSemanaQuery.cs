using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DiaSemana;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class GetDiaSemanaQuery : QueryBase<GetDiaSemanaDto>
    {
        public GetDiaSemanaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
