using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public string? Descripcion { get; set; }

    public int IdTipoProveedor { get; set; }

    public int IdEstadoProveedor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();

    public virtual ICollection<GananciaProveedor> GananciaProveedors { get; set; } = new List<GananciaProveedor>();

    public virtual EstadoProveedor IdEstadoProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdProveedorNavigation { get; set; } = null!;

    public virtual TipoProveedor IdTipoProveedorNavigation { get; set; } = null!;
}
