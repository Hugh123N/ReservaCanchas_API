using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class GetHorarioCanchaQuery : QueryBase<GetHorarioCanchaDto>
    {
        public GetHorarioCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
