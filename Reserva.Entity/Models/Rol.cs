using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
