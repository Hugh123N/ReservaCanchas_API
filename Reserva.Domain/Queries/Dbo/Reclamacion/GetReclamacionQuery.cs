using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Domain.Queries.Dbo.Reclamacion
{
    public class GetReclamacionQuery : QueryBase<GetReclamacionDto>
    {
        public GetReclamacionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
