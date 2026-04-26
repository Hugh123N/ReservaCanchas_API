using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class ComprobantePagoPlan
{
    public int IdComprobantePagoPlan { get; set; }

    public int IdPagoPlan { get; set; }

    public string? TipoComprobante { get; set; }

    public string? Serie { get; set; }

    public string? Numero { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public string? Direccion { get; set; }

    public string? UrlPdf { get; set; }

    public string? UrlXml { get; set; }

    public DateTimeOffset FechaEmision { get; set; }

    public string? EstadoSunat { get; set; }

    public string? Hash { get; set; }

    public bool Activo { get; set; }

    public virtual PagoPlan IdPagoPlanNavigation { get; set; } = null!;
}
