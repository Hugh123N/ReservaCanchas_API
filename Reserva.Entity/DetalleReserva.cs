using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class DetalleReserva
{
    public int IdDetalleReserva { get; set; }

    public int IdReserva { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public decimal DuracionHoras { get; set; }

    public decimal PrecioHora { get; set; }

    public decimal Subtotal { get; set; }

    public bool Activo { get; set; }

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
