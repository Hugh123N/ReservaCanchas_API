using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class ProveedorPlan
{
    public int IdProveedorPlan { get; set; }

    public int IdProveedor { get; set; }

    public int IdPlane { get; set; }

    public int IdPlanTarifa { get; set; }

    public DateTimeOffset FechaInicio { get; set; }

    public DateTimeOffset FechaFin { get; set; }

    public DateTimeOffset? FechaProximoCobro { get; set; }

    public string Estado { get; set; } = null!;

    public bool AutoRenovacion { get; set; }

    public bool EsActual { get; set; }

    public string? CulqiSubscriptionId { get; set; }

    public string? CulqiSubscriptionIdAnterior { get; set; }

    public string? CulqiCustomerId { get; set; }

    public DateTimeOffset? GracePeriodHasta { get; set; }

    public DateTimeOffset? FechaCancelacion { get; set; }

    public string? MotivoCancelacion { get; set; }
   
    public bool CancelAtPeriodEnd { get; set; }

    public bool EsPruebaGratis { get; set; }

    public decimal SaldoFavor { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public bool Activo { get; set; }

    public virtual PlanTarifa IdPlanTarifaNavigation { get; set; } = null!;

    public virtual Plane IdPlaneNavigation { get; set; } = null!;

    public virtual ICollection<PagoPlan> PagoPlan { get; set; } = new List<PagoPlan>();
}
