using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Usuario : IdentityUser<Guid>
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Apellidos { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Telefono { get; set; }

    public string? Imagen { get; set; }

    public int IdRol { get; set; }

    public int IdEstadoUsuario { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CanchaFavorita> CanchaFavorita { get; set; } = new List<CanchaFavorita>();

    public virtual EstadoUsuario IdEstadoUsuarioNavigation { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<IntentoLogin> IntentoLogins { get; set; } = new List<IntentoLogin>();

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual Proveedor? Proveedor { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
