using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Domain.Queries.Dbo.Ubigeo
{
    public class GetUbigeoQuery : QueryBase<GetUbigeoDto>
    {
        public GetUbigeoQuery(string id) => Id = id;
        public string Id { get; set; }
    }
}
