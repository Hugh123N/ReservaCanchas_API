using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class TipoProveedor
{
    public int IdTipoProveedor { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();
}
