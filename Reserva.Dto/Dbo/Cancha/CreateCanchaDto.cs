using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Dto.Dbo.Cancha
{
    public class CreateCanchaDto : CanchaDto
    {
        public List<CreateHorarioCanchaDto> HorarioCanchas { get; set; } = new List<CreateHorarioCanchaDto>();
        public List<int> IdsTipoDeportes { get; set; } = new List<int>();
        public List<int> IdsServicios { get; set; } = new List<int>();
    }
}
