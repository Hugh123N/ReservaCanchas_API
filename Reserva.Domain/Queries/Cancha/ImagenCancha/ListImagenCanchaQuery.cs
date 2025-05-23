using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class ListImagenCanchaQuery : QueryBase<IEnumerable<ListImagenCanchaDto>>
    {
        public ListImagenCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
