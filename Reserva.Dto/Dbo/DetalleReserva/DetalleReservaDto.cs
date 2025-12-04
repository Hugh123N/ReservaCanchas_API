using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.DetalleReserva;

public class DetalleReservaDto
{

    public int IdReserva { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public decimal DuracionHoras { get; set; }

    public decimal PrecioHora { get; set; }

    public decimal Subtotal { get; set; }
}
