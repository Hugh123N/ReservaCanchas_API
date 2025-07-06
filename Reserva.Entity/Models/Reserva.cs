using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int IdUsuario { get; set; }

    public int IdCancha { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public int IdEstadoReserva { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<GananciaProveedor> GananciaProveedors { get; set; } = new List<GananciaProveedor>();

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual EstadoReserva IdEstadoReservaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
