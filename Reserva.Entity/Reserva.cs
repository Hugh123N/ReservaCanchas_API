using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public string CodigoReserva { get; set; } = null!;

    public Guid IdUsuario { get; set; }

    public int IdCancha { get; set; }

    public DateTimeOffset Fecha { get; set; }

    public decimal? Monto { get; set; }

    public DateTimeOffset? FechaExpiracionPreReserva { get; set; }

    public int IdEstadoReserva { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual EstadoReserva IdEstadoReservaNavigation { get; set; } = null!;

    public virtual AspNetUsers IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();

    public virtual ICollection<ReservaDetalle> ReservaDetalle { get; set; } = new List<ReservaDetalle>();
}
