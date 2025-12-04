using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class ConfiguracionProveedor
{
    public int IdConfiguracionProveedor { get; set; }

    public int IdProveedor { get; set; }

    public int? DuracionPreReserva { get; set; }

    public decimal PorcentajeAdelantoMinimo { get; set; }

    public int TiempoLimiteCancelacion { get; set; }

    public decimal PorcentajeDevolucionCompleto { get; set; }

    public decimal PorcentajeDevolucionParcial { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
