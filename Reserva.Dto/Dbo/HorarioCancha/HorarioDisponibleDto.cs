using System;

namespace Reserva.Dto.Dbo.HorarioCancha;

public class HorarioDisponibleDto
{
    public int? IdHorarioCancha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public decimal? Precio { get; set; } 
}
