using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class UsoPlan
{
    public int IdUsoPlan { get; set; }

    public int IdProveedor { get; set; }

    public string Codigo { get; set; } = null!;

    public int ValorActual { get; set; }

    public bool Activo { get; set; }
}
