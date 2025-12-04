using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Icono { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ServicioCancha> ServicioCancha { get; set; } = new List<ServicioCancha>();
}
