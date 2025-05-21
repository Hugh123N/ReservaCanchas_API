using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class DiaSemana
{
    public int IdDia { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<Disponibilidad> Disponibilidads { get; set; } = new List<Disponibilidad>();
}
