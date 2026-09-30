using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class EstadoPago
{
    public int IdEstadoPago { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
