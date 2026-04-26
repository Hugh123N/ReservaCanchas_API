using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Plane
{
    public int IdPlane { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? OrdenVisual { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<PlanCaracteristica> PlanCaracteristica { get; set; } = new List<PlanCaracteristica>();

    public virtual ICollection<PlanLimite> PlanLimite { get; set; } = new List<PlanLimite>();

    public virtual ICollection<PlanTarifa> PlanTarifa { get; set; } = new List<PlanTarifa>();

    public virtual ICollection<ProveedorPlan> ProveedorPlan { get; set; } = new List<ProveedorPlan>();
}
