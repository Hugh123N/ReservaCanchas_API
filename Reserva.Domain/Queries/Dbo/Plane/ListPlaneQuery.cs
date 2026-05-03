using Reserva.Dto.Dbo.Plane;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Plane
{
    public class ListPlaneQuery : QueryBase<IEnumerable<ListPlaneDto>>
    {
        public ListPlaneQuery() { }
    }
}
