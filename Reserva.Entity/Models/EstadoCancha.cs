using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class EstadoCancha
{
    public int IdEstadoCancha { get; set; }

    public string? Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();
}
