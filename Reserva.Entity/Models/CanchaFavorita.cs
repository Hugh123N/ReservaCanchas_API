using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class CanchaFavorita
{
    public int IdUsuario { get; set; }

    public int IdCancha { get; set; }

    public DateTimeOffset FechaAgregado { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
