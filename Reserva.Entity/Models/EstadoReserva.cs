using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class EstadoReserva
{
    public int IdEstadoReserva { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
