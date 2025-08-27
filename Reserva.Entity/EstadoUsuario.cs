using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class EstadoUsuario
{
    public int IdEstadoUsuario { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<AspNetUsers> AspNetUsers { get; set; } = new List<AspNetUsers>();
}
