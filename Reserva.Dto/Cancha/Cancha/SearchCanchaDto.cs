
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Dto.Cancha.Cancha
{
    public class SearchCanchaDto: CanchaDto
    {
        public int? IdCancha { get; set; }
        public GetTipoCanchaDto? TipoCancha { get; set; }
        public List<GetImagenCanchaDto>? ImagenesCancha { get; set; }
    }
}
