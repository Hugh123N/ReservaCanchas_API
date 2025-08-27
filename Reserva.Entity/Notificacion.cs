using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public Guid IdUsuario { get; set; }

    public string Mensaje { get; set; } = null!;

    public bool? Leido { get; set; }

    public DateTimeOffset? FechaCreacion { get; set; }

    public bool Activo { get; set; }

    public virtual AspNetUsers IdUsuarioNavigation { get; set; } = null!;
}
