using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class EstadoReserva
{
    public int IdEstado { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
