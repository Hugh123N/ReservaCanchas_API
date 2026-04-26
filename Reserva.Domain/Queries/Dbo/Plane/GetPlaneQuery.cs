using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Domain.Queries.Dbo.Plane
{
    public class GetPlaneQuery : QueryBase<GetPlaneDto>
    {
        public GetPlaneQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
