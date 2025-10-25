using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Domain.Queries.Dbo.ImagenCancha
{
    public class GetImagenCanchaQuery : QueryBase<GetImagenCanchaDto>
    {
        public GetImagenCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
