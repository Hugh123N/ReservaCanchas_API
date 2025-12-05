using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class TipoDeporteCancha
{
    public int IdTipoDeporteCancha { get; set; }

    public int IdCancha { get; set; }

    public int IdTipoDeporte { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual TipoDeporte IdTipoDeporteNavigation { get; set; } = null!;
}
