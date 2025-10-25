using Reserva.Dto.Dbo.Disponibilidad;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Dto.Dbo.Cancha
{
    public class UpdateCanchaDto : CanchaDto
    {
        public int IdCancha { get; set; }
        public List<UpdateImagenCanchaDto>? Imagenes { get; set; }
        public List<UpdateDisponibilidadDto> Disponibilidades { get; set; } = new List<UpdateDisponibilidadDto>();
    }
}
