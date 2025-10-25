using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.ImagenCancha
{
    public class ListImagenCanchaQuery : QueryBase<IEnumerable<ListImagenCanchaDto>>
    {
        public ListImagenCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
