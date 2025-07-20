using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class EstadoUsuario
{
    public int IdEstadoUsuario { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<AspNetUser> AspNetUsers { get; set; } = new List<AspNetUser>();
}
