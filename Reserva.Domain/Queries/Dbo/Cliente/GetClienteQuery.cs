using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Domain.Queries.Dbo.Cliente
{
    public class GetClienteQuery : QueryBase<GetClienteDto>
    {
        public GetClienteQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
