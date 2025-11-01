using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class ReservaDetalle
{
    public int IdReservaDetalle { get; set; }

    public int IdReserva { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
