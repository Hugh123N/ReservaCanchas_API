using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public Guid IdUsuario { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public int IdTipoProveedor { get; set; }

    public int IdEstadoProveedor { get; set; }

    public string? Telefono { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? CulqiCustomerId { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Cancha { get; set; } = new List<Cancha>();

    public virtual ConfiguracionProveedor? ConfiguracionProveedor { get; set; }

    public virtual EstadoProveedor IdEstadoProveedorNavigation { get; set; } = null!;

    public virtual TipoProveedor IdTipoProveedorNavigation { get; set; } = null!;

    public virtual AspNetUsers IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Operador> Operador { get; set; } = new List<Operador>();
}
