using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Cancha
{
    public int IdCancha { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdTipo { get; set; }

    public string? Descripcion { get; set; }

    public string? Ubicacion { get; set; }

    public string? Imagen { get; set; }

    public decimal? PrecioHora { get; set; }

    public bool? Activa { get; set; }

    public virtual ICollection<Disponibilidad> Disponibilidads { get; set; } = new List<Disponibilidad>();

    public virtual Tipo IdTipoNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
