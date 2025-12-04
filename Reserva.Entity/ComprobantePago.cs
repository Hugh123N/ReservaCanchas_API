using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class ComprobantePago
{
    public int IdComprobantePago { get; set; }

    public int IdPago { get; set; }

    public string NumeroComprobante { get; set; } = null!;

    public string TipoComprobante { get; set; } = null!;

    public string? UrlPdf { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public virtual Pago IdPagoNavigation { get; set; } = null!;
}
