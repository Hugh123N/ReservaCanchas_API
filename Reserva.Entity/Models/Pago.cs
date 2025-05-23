using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdUsuario { get; set; }

    public decimal Monto { get; set; }

    public int? IdMetodoPago { get; set; }

    public int IdEstadoPago { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Activo { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
