using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class TipoCancha
{
    public int IdTipoCancha { get; set; }

    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Cancha { get; set; } = new List<Cancha>();
}
