using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoCancha;

namespace Reserva.Domain.Queries.Dbo.TipoCancha
{
    public class GetTipoCanchaQuery : QueryBase<GetTipoCanchaDto>
    {
        public GetTipoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
