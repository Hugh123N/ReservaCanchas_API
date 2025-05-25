using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Domain.Queries.Cancha.Ubigeo
{
    public class GetUbigeoQuery : QueryBase<GetUbigeoDto>
    {
        public GetUbigeoQuery(string id) => Id = id;
        public string Id { get; set; }
    }
}
