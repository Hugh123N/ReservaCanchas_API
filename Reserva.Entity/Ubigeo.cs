using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Ubigeo
{
    public string CodigoUbigeo { get; set; } = null!;

    public string? Departamento { get; set; }

    public string? Provincia { get; set; }

    public string? Distrito { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cancha> Cancha { get; set; } = new List<Cancha>();
}
