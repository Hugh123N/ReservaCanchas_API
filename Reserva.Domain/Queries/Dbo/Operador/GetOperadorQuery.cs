using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class GetOperadorQuery : QueryBase<GetOperadorDto>
    {
        public GetOperadorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
