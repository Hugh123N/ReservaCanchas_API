using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }

    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; }

    public virtual ICollection<Provincia> Provincia { get; set; } = new List<Provincia>();
}
