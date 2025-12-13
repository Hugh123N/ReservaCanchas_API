using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Dto.Dbo.Cancha
{
    public class UpdateCanchaDto : CanchaDto
    {
        public int IdEstadoCancha { get; set; }
        public int IdCancha { get; set; }
        public List<UpdateHorarioCanchaDto> HorarioCanchas { get; set; } = new List<UpdateHorarioCanchaDto>();
        public List<int> IdsTipoDeportes { get; set; } = new List<int>();
        public List<int> IdsServicios { get; set; } = new List<int>();
    }
}
