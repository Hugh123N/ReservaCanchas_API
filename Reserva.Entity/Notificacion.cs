using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public Guid IdUsuario { get; set; }

    public string Titulo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string? Tipo { get; set; }

    public bool? Leido { get; set; }

    public int? IdReserva { get; set; }

    public DateTimeOffset? FechaCreacion { get; set; }

    public DateTimeOffset? FechaLeido { get; set; }

    public bool Activo { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }

    public virtual AspNetUsers IdUsuarioNavigation { get; set; } = null!;
}
