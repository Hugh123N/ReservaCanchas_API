using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class GetDistritoQuery : QueryBase<GetDistritoDto>
    {
        public GetDistritoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
