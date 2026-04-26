using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public string Modulo { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string Canal { get; set; } = null!;

    public string? Destinatario { get; set; }

    public DateTimeOffset? FechaProgramada { get; set; }

    public DateTimeOffset? FechaEnvio { get; set; }

    public int Intentos { get; set; }

    public Guid? IdUsuario { get; set; }

    public int? IdProveedor { get; set; }

    public int? IdReserva { get; set; }

    public int? IdPago { get; set; }

    public int? IdProveedorPlan { get; set; }

    public string? Metadata { get; set; }

    public string? UserNameCreate { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public bool Activo { get; set; }
}
