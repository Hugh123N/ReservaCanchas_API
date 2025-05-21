using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Distrito
{
    public int IdDistrito { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdProvincia { get; set; }

    public virtual Provincium IdProvinciaNavigation { get; set; } = null!;

    public virtual ICollection<Zona> Zonas { get; set; } = new List<Zona>();
}
