using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Domain.Queries.Dbo.TipoSuperficie
{
    public class GetTipoSuperficieQuery : QueryBase<GetTipoSuperficieDto>
    {
        public GetTipoSuperficieQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
