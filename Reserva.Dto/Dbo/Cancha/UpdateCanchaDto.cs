using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Dto.Dbo.Cancha
{
    public class UpdateCanchaDto : CanchaDto
    {
        public int IdCancha { get; set; }
        public List<UpdateImagenCanchaDto>? Imagenes { get; set; }
        public List<UpdateHorarioCanchaDto> HorarioCanchas { get; set; } = new List<UpdateHorarioCanchaDto>();
    }
}
