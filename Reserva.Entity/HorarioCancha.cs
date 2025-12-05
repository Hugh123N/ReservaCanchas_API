using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class HorarioCancha
{
    public int IdHorarioCancha { get; set; }

    public int IdCancha { get; set; }

    public int IdDiaSemana { get; set; }

    public int IdHoraInicio { get; set; }

    public int? IdHoraFin { get; set; }

    public decimal PrecioHora { get; set; }

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<DetalleReserva> DetalleReserva { get; set; } = new List<DetalleReserva>();

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual DiaSemana IdDiaSemanaNavigation { get; set; } = null!;

    public virtual Hora? IdHoraFinNavigation { get; set; }

    public virtual Hora IdHoraInicioNavigation { get; set; } = null!;
}
