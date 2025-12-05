using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class BloqueoHorario
{
    public int IdBloqueoHorario { get; set; }

    public int IdCancha { get; set; }

    public DateOnly FechaBloqueo { get; set; }

    public int IdHoraInicio { get; set; }

    public int? IdHoraFin { get; set; }

    public string? Motivo { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual Hora? IdHoraFinNavigation { get; set; }

    public virtual Hora IdHoraInicioNavigation { get; set; } = null!;
}
