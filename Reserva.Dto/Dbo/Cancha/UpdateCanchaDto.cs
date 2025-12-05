using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Dto.Dbo.Cancha
{
    public class UpdateCanchaDto : CanchaDto
    {
        public int IdCancha { get; set; }
        public List<UpdateImagenCanchaDto>? Imagenes { get; set; }
        public List<UpdateHorarioCanchaDto> HorarioCanchas { get; set; } = new List<UpdateHorarioCanchaDto>();
        public List<int> IdsTipoDeporte { get; set; } = new List<int>();
        public List<int> IdsServicios { get; set; } = new List<int>();
    }
}
