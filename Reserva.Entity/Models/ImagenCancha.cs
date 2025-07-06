using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class ImagenCancha
{
    public int IdImagenCancha { get; set; }

    public int IdCancha { get; set; }

    public string UrlImagen { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;
}
