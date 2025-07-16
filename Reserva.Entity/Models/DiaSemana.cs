using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class DiaSemana
{
    public int IdDiaSemana { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }
}
