using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class PagoPlan
{
    public int IdPagoPlan { get; set; }

    public int IdProveedorPlan { get; set; }

    public decimal Monto { get; set; }

    public string? Moneda { get; set; }

    public int IdMetodoPago { get; set; }

    public int IdEstadoPago { get; set; }

    public DateTimeOffset? FechaPago { get; set; }

    public string? CulqiChargeId { get; set; }

    public string? CodigoOperacion { get; set; }

    public string? RespuestaGateway { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ComprobantePagoPlan> ComprobantePagoPlan { get; set; } = new List<ComprobantePagoPlan>();

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;

    public virtual ProveedorPlan IdProveedorPlanNavigation { get; set; } = null!;
}
