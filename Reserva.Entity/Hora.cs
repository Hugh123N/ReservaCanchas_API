using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Hora
{
    public int IdHora { get; set; }

    public TimeOnly Hora1 { get; set; }

    public string HoraTexto { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<BloqueoHorario> BloqueoHorarioIdHoraFinNavigation { get; set; } = new List<BloqueoHorario>();

    public virtual ICollection<BloqueoHorario> BloqueoHorarioIdHoraInicioNavigation { get; set; } = new List<BloqueoHorario>();

    public virtual ICollection<HorarioCancha> HorarioCanchaIdHoraFinNavigation { get; set; } = new List<HorarioCancha>();

    public virtual ICollection<HorarioCancha> HorarioCanchaIdHoraInicioNavigation { get; set; } = new List<HorarioCancha>();
}
