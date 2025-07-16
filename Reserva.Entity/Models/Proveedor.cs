using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Proveedor
{
    public int IdUsuario { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public int IdTipoProveedor { get; set; }

    public int IdEstadoProveedor { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual EstadoProveedor IdEstadoProveedorNavigation { get; set; } = null!;

    public virtual TipoProveedor IdTipoProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
