using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Operador
{
    public int IdOperador { get; set; }

    public Guid IdUsuario { get; set; }

    public int IdProveedor { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual AspNetUsers IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<OperadorCancha> OperadorCancha { get; set; } = new List<OperadorCancha>();
}
