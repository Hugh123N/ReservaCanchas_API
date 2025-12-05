using System;

namespace Reserva.Dto.Dbo.HorarioCancha;

public class HorarioDisponibleDto
{
    public TimeOnly Hora { get; set; }

    public string HoraTexto { get; set; } = null!;

    public decimal Precio { get; set; }
}
