using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class DetallePago
{
    public int IdDetallePago { get; set; }

    public int? IdPago { get; set; }

    public int? IdReserva { get; set; }
    public bool Activo { get; set; }

    public virtual Pago? IdPagoNavigation { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }
}
