using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Dto.Cancha.Cancha
{
    public class GetCanchaDto : CanchaDto
    {
        public int IdCancha { get; set; }
        public bool Activo { get; set; }

        public GetTipoCanchaDto? TipoCancha { get; set; }
        public List<GetImagenCanchaDto>? ImagenesCancha { get; set; }
        public GetEstadoCanchaDto? EstadoCancha { get; set; }
        public List<GetCanchaFavoritaDto>? Faboritos { get; set; }
        public GetUbigeoDto? Ubigeo { get; set; }
        public List<string>? Disponibilidad { get; set; }
    }
}
