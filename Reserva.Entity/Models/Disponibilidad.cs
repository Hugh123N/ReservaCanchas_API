using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Disponibilidad
{
    public int Id { get; set; }

    public int IdCancha { get; set; }

    public int IdDia { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual DiaSemana IdDiaNavigation { get; set; } = null!;
}
