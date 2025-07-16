using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class EstadoReserva
{
    public int IdEstadoReserva { get; set; }

    public string? Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }
}
