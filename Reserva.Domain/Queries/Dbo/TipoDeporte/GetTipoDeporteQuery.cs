using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Domain.Queries.Dbo.TipoDeporte
{
    public class GetTipoDeporteQuery : QueryBase<GetTipoDeporteDto>
    {
        public GetTipoDeporteQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
