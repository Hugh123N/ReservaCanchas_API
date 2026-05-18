
using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Dto.Dbo.EstadoCancha;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Dto.Dbo.Cancha
{
    public class SearchCanchaDto: CanchaDto
    {
        public string Codigo { get; set; } = null!;
        public int? IdCancha { get; set; }
        public string? UrlImagen { get; set; }
        public List<GetTipoDeporteDto>? TipoDeportes { get; set; }
        public GetEstadoCanchaDto? EstadoCancha { get; set; }
        public List<GetCanchaFavoritaDto>? Faboritos { get; set; }
        public GetUbigeoDto? Ubigeo { get; set; }
    }
}
