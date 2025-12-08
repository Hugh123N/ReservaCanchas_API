using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class GetHoraQuery : QueryBase<GetHoraDto>
    {
        public GetHoraQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
