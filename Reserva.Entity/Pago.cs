using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Pago
{
    public int IdPago { get; set; }

    public int? IdReserva { get; set; }

    public string Moneda { get; set; } = null!;

    public decimal Monto { get; set; }

    public decimal MontoAdelanto { get; set; }

    public decimal MontoPendiente { get; set; }

    public int Revision { get; set; }

    public decimal? MontoReembolso { get; set; }

    public int IdEstadoPago { get; set; }

    public string? CulqiChargeId { get; set; }

    public string? CulqiTokenId { get; set; }

    public string? CulqiReferenceCode { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ComprobantePago> ComprobantePago { get; set; } = new List<ComprobantePago>();

    public virtual ICollection<MovimientoPago> MovimientoPago { get; set; } = new List<MovimientoPago>();

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;

    public virtual Reserva? IdReservaNavigation { get; set; }
}
