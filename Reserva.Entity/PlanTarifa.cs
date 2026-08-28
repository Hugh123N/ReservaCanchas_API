using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class PlanTarifa
{
    public int IdPlanTarifa { get; set; }

    public int IdPlane { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public decimal Precio { get; set; }

    public string Moneda { get; set; } = null!;

    public int? DuracionDias { get; set; }

    public decimal? PorcentajeDescuento { get; set; }

    public string TipoCobro { get; set; } = null!;

    public bool? PermiteAutoRenovacion { get; set; }
    public string? IdPlanCulqi { get; set; }

    public bool Activo { get; set; }

    public virtual Plane IdPlaneNavigation { get; set; } = null!;

    public virtual ICollection<ProveedorPlan> ProveedorPlan { get; set; } = new List<ProveedorPlan>();
}
