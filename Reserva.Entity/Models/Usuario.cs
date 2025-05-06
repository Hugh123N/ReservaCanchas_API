using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Apellidos { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Telefono { get; set; }

    public string? Imagen { get; set; }

    public int IdRol { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Reserva> ReservaCreadoPorNavigations { get; set; } = new List<Reserva>();

    public virtual ICollection<Reserva> ReservaIdUsuarioNavigations { get; set; } = new List<Reserva>();
}
