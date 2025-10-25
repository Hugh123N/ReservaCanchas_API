using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Comision;

namespace Reserva.Domain.Queries.Dbo.Comision
{
    public class GetComisionQuery : QueryBase<GetComisionDto>
    {
        public GetComisionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
