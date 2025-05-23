using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class GetTipoCanchaQuery : QueryBase<GetTipoCanchaDto>
    {
        public GetTipoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
