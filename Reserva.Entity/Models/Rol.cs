using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Rol : IdentityRole<Guid>
{
    public int IdRol { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
