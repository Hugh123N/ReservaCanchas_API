using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class CanchaFavorita
{
    public Guid IdUsuario { get; set; }

    public int IdCancha { get; set; }

    public DateTimeOffset FechaAgregado { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
}
