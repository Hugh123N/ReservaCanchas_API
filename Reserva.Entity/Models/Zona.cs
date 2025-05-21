using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Zona
{
    public int IdZona { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdDistrito { get; set; }

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();

    public virtual Distrito IdDistritoNavigation { get; set; } = null!;
}
