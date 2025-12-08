using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Dto.Dbo.HorarioCancha
{
    public class GetHorarioCanchaDto : HorarioCanchaDto
    {
        public int IdHorarioCancha { get; set; }
        public GetDiaSemanaDto? DiaSemana { get; set; }
        public GetHoraDto? HoraInicio { get; set; }
    }
}
