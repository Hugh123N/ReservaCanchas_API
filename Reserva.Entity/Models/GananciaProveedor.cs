using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class GananciaProveedor
{
    public int IdGananciaProveedor { get; set; }

    public int IdReserva { get; set; }

    public int IdProveedor { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal Comision { get; set; }

    public decimal GananciaNeta { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Activo { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
