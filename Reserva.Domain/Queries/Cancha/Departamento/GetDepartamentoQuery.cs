using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class GetDepartamentoQuery : QueryBase<GetDepartamentoDto>
    {
        public GetDepartamentoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
