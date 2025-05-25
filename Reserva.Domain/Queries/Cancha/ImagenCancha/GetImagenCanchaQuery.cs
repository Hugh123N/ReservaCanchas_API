using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class GetImagenCanchaQuery : QueryBase<GetImagenCanchaDto>
    {
        public GetImagenCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
