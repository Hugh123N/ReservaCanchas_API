using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Provincia
{
    public int IdProvincia { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdDepartamento { get; set; }

    public virtual ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();

    public virtual Departamento IdDepartamentoNavigation { get; set; } = null!;
}
