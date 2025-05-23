using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class GetProvinciaQuery : QueryBase<GetProvinciaDto>
    {
        public GetProvinciaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
