using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class ServicioCancha
{
    public int IdServicioCancha { get; set; }

    public int IdCancha { get; set; }

    public int IdServicio { get; set; }

    public bool EsIncluido { get; set; }

    public decimal? CostoAdicional { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
