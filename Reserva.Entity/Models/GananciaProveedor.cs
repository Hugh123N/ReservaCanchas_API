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

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
