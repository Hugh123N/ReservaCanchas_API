using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class PlanCaracteristica
{
    public int IdPlanCaracteristica { get; set; }

    public int IdPlane { get; set; }

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; }

    public virtual Plane IdPlaneNavigation { get; set; } = null!;
}
