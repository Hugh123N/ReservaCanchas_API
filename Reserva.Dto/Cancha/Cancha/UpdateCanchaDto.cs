using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Dto.Cancha.Cancha
{
    public class UpdateCanchaDto : CanchaDto
    {
        public int IdCancha { get; set; }
        public List<UpdateImagenCanchaDto>? Imagenes { get; set; }
        public List<UpdateDisponibilidadDto> Disponibilidades { get; set; } = new List<UpdateDisponibilidadDto>();
    }
}
