using Reserva.Dto.Cancha.Departamento;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class ListDepartamentoQuery : QueryBase<IEnumerable<ListDepartamentoDto>>
    {
        public ListDepartamentoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
