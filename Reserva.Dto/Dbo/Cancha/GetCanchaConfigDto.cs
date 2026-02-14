using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Dto.Dbo.Cancha
{
    public class GetCanchaConfigDto
    {
        public List<GetTipoDeporteDto>? TipoDeportes { get; set; }
        public decimal? PorcentajeAdelantoMinimo { get; set; }
    }
}
