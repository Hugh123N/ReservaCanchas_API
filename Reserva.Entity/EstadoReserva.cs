using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class EstadoReserva
{
    public int IdEstadoReserva { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();
}
