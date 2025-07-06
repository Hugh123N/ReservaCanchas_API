using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Comision
{
    public int IdComision { get; set; }

    public decimal Porcentaje { get; set; }

    public DateTimeOffset? FechaInicio { get; set; }

    public DateTimeOffset? FechaFin { get; set; }

    public DateTimeOffset? FechaActualizacion { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }
}
