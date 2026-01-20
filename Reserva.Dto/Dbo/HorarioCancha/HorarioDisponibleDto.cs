using System;

namespace Reserva.Dto.Dbo.HorarioCancha;

public class HorarioDisponibleDto
{
    public int IdHorarioCancha { get; set; }
    public TimeOnly HoraInicio { get; set; }

    public string HoraInicioTexto { get; set; } = null!;

    public TimeOnly HoraFin { get; set; }

    public string HoraFinTexto { get; set; } = null!;

    public decimal Precio { get; set; }
}
