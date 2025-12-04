using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class TipoDeporte
{
    public int IdTipoDeporte { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Icono { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();

    public virtual ICollection<TipoDeporteCancha> TipoDeporteCancha { get; set; } = new List<TipoDeporteCancha>();
}
