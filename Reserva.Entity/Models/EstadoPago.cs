using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class EstadoPago
{
    public int IdEstadoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
