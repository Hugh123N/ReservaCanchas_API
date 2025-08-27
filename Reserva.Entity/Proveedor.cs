using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Proveedor
{
    public Guid IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public int IdTipoProveedor { get; set; }

    public int IdEstadoProveedor { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Cancha { get; set; } = new List<Cancha>();

    public virtual ICollection<GananciaProveedor> GananciaProveedor { get; set; } = new List<GananciaProveedor>();

    public virtual EstadoProveedor IdEstadoProveedorNavigation { get; set; } = null!;

    public virtual AspNetUsers IdProveedorNavigation { get; set; } = null!;

    public virtual TipoProveedor IdTipoProveedorNavigation { get; set; } = null!;
}
