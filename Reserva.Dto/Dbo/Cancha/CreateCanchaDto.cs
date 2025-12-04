using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Dto.Dbo.Cancha
{
    public class CreateCanchaDto : CanchaDto
    {
        public List<CreateImagenCanchaDto>? Imagenes { get; set; }
        public List<CreateHorarioCanchaDto> HorarioCanchas { get; set; } = new List<CreateHorarioCanchaDto>();

    }
}
