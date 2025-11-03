using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Pago
{
    public int IdPago { get; set; }

    public int? IdReserva { get; set; }

    public int? IdPlan { get; set; }

    public string Moneda { get; set; } = null!;

    public string? CodigoOperacion { get; set; }

    public decimal Monto { get; set; }

    public decimal? MontoAdelanto { get; set; }

    public decimal? MontoPendiente { get; set; }

    public string? NumeroReferencia { get; set; }

    public int? IdMetodoPago { get; set; }

    public int IdEstadoPago { get; set; }

    public string? CulqiChargeId { get; set; }

    public string? CulqiTokenId { get; set; }

    public string? CulqiReferenceCode { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }
}
