using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class PlanLimite
{
    public int IdPlanLimite { get; set; }

    public int IdPlane { get; set; }

    public string Codigo { get; set; } = null!;

    public int Valor { get; set; }

    public bool Activo { get; set; }

    public virtual Plane IdPlaneNavigation { get; set; } = null!;
}
