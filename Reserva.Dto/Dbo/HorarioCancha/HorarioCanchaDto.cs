using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.HorarioCancha;

public class HorarioCanchaDto
{

    public int IdCancha { get; set; }

    public int IdDiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public decimal PrecioHora { get; set; }






}
