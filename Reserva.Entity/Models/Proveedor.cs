using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();

    public virtual ICollection<GananciaProveedor> GananciaProveedors { get; set; } = new List<GananciaProveedor>();

    public virtual Usuario IdProveedorNavigation { get; set; } = null!;
}
