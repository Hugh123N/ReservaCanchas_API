using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Pago
{
    public int IdPago { get; set; }

    public Guid IdUsuario { get; set; }

    public decimal Monto { get; set; }

    public int? IdMetodoPago { get; set; }

    public int IdEstadoPago { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    public virtual AspNetUsers IdUsuarioNavigation { get; set; } = null!;
}
