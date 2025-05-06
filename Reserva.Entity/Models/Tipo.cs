using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Tipo
{
    public int IdTipo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();
}
