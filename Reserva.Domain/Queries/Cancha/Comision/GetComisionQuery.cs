using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class GetComisionQuery : QueryBase<GetComisionDto>
    {
        public GetComisionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
