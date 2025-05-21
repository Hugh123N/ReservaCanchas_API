using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class ImagenCancha
{
    public int IdImagen { get; set; }

    public int IdCancha { get; set; }

    public string UrlImagen { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;
}
