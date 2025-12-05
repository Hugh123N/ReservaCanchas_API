using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class DetalleReserva
{
    public int IdDetalleReserva { get; set; }

    public int IdReserva { get; set; }

    public int? IdHorarioCancha { get; set; }

    public bool Activo { get; set; }

    public virtual HorarioCancha? IdHorarioCanchaNavigation { get; set; }

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
