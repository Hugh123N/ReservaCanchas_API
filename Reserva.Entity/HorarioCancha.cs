using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class HorarioCancha
{
    public int IdHorarioCancha { get; set; }

    public int IdCancha { get; set; }

    public int IdDiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public decimal PrecioHora { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual DiaSemana IdDiaSemanaNavigation { get; set; } = null!;
}
