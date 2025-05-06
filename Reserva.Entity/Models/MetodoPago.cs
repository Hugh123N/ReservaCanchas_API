using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class MetodoPago
{
    public int IdMetodo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
