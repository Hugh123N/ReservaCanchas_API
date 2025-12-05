using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public string CodigoReserva { get; set; } = null!;

    public Guid IdCliente { get; set; }

    public int IdCancha { get; set; }

    public int IdTipoDeporte { get; set; }

    public DateTimeOffset FechaReserva { get; set; }

    public decimal MontoTotal { get; set; }

    public DateTimeOffset? FechaExpiracionPreReserva { get; set; }

    public bool? NotificacionAdvertenciaEnviada { get; set; }

    public bool? RecordatorioEnviado { get; set; }

    public int IdEstadoReserva { get; set; }

    public int? IdOperadorConfirmo { get; set; }

    public DateTimeOffset? FechaConfirmacion { get; set; }

    public string? Observaciones { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<DetalleReserva> DetalleReserva { get; set; } = new List<DetalleReserva>();

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual AspNetUsers IdClienteNavigation { get; set; } = null!;

    public virtual EstadoReserva IdEstadoReservaNavigation { get; set; } = null!;

    public virtual Operador? IdOperadorConfirmoNavigation { get; set; }

    public virtual TipoDeporte IdTipoDeporteNavigation { get; set; } = null!;

    public virtual ICollection<Notificacion> Notificacion { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
