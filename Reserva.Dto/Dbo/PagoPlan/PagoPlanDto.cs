using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.PagoPlan;

public class PagoPlanDto
{

    public int IdProveedorPlan { get; set; }

    public decimal Monto { get; set; }

    public string? Moneda { get; set; }

    public int IdMetodoPago { get; set; }

    public int IdEstadoPago { get; set; }

    public DateTimeOffset? FechaPago { get; set; }

    public string? CulqiChargeId { get; set; }

    public string? CodigoOperacion { get; set; }

    public string? RespuestaGateway { get; set; }
}
