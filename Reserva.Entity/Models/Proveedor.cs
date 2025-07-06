using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public int IdTipoProveedor { get; set; }

    public int IdEstadoProveedor { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();

    public virtual ICollection<GananciaProveedor> GananciaProveedors { get; set; } = new List<GananciaProveedor>();

    public virtual EstadoProveedor IdEstadoProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdProveedorNavigation { get; set; } = null!;

    public virtual TipoProveedor IdTipoProveedorNavigation { get; set; } = null!;
}
