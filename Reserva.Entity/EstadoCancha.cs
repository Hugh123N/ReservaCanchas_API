using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class EstadoCancha
{
    public int IdEstadoCancha { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Cancha { get; set; } = new List<Cancha>();
}
