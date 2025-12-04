using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Dto.Dbo.EstadoCancha;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Dto.Dbo.Cancha
{
    public class GetCanchaDto : CanchaDto
    {
        public int IdCancha { get; set; }

        /// <summary>
        /// Duración en horas de la pre-reserva antes de expirar
        /// </summary>
        public int? DuracionPreReserva { get; set; }

        /// <summary>
        /// Porcentaje mínimo de adelanto requerido para confirmar reserva (0-100)
        /// </summary>
        public decimal? PorcentajeAdelanto { get; set; }

        public List<GetTipoDeporteDto>? TipoDeportes { get; set; }
        public List<GetImagenCanchaDto>? ImagenesCancha { get; set; }
        public GetEstadoCanchaDto? EstadoCancha { get; set; }
        public List<GetCanchaFavoritaDto>? Faboritos { get; set; }
        public GetUbigeoDto? Ubigeo { get; set; }
        public List<string>? HorariosDisponibles { get; set; }
    }
}
