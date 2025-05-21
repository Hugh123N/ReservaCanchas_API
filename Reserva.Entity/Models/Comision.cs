using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Comision
{
    public int IdComision { get; set; }

    public decimal Porcentaje { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public bool? Activo { get; set; }
}
