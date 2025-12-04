using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class OperadorCancha
{
    public int IdOperadorCancha { get; set; }

    public int IdOperador { get; set; }

    public int IdCancha { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual Operador IdOperadorNavigation { get; set; } = null!;
}
