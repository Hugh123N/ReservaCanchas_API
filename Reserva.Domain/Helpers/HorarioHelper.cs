using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Helpers
{
    /// <summary>
    /// Helper genérico para operaciones de agrupación y formateo de horarios.
    /// Centraliza la lógica de unión de horarios consecutivos.
    /// </summary>
    public static class HorarioHelper
    {
        /// <summary>
        /// Convierte entidades HorarioCancha en HorarioDisponibleDto agrupando bloques consecutivos.
        /// Ejemplo: [06:00-06:30, 06:30-07:00, 08:00-08:30, 08:30-09:00]
        ///       → [06:00-07:00, 08:00-09:00]
        /// </summary>
        public static List<HorarioDisponibleDto> AgruparHorarios(List<Entity.HorarioCancha> horarios)
        {
            if (horarios == null || !horarios.Any())
                return new List<HorarioDisponibleDto>();

            var horariosDto = horarios
                .Where(h => h.IdHoraInicioNavigation != null && h.IdHoraFinNavigation != null)
                .Select(h => new HorarioDisponibleDto
                {
                    IdHorarioCancha = h.IdHorarioCancha,
                    HoraInicio = h.IdHoraInicioNavigation!.Hora1,
                    HoraFin = h.IdHoraFinNavigation!.Hora1,
                    Precio = h.PrecioHora
                })
                .OrderBy(h => h.HoraInicio)
                .ToList();

            return UnirHorariosConsecutivos(horariosDto);
        }

        /// <summary>
        /// Convierte detalles de reserva (con navegaciones a HorarioCancha) en HorarioDisponibleDto agrupados.
        /// </summary>
        public static List<HorarioDisponibleDto> AgruparHorariosDesdeDetalles(List<Entity.DetalleReserva> detalles)
        {
            if (detalles == null || !detalles.Any())
                return new List<HorarioDisponibleDto>();

            var horariosDto = detalles
                .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                         && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                .Select(d => new HorarioDisponibleDto
                {
                    IdHorarioCancha = d.IdHorarioCancha,
                    HoraInicio = d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1,
                    HoraFin = d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1,
                    Precio = d.IdHorarioCanchaNavigation!.PrecioHora
                })
                .OrderBy(h => h.HoraInicio)
                .ToList();

            return UnirHorariosConsecutivos(horariosDto);
        }

        /// <summary>
        /// Formatea horarios agrupando los consecutivos para notificaciones.
        /// Ejemplo: 08:00-09:00, 09:00-10:00, 10:00-11:00 → "08:00 - 11:00 (3 horas)"
        /// </summary>
        public static string FormatearHorariosConsecutivos(List<(TimeOnly inicio, TimeOnly fin)> horarios)
        {
            if (horarios == null || !horarios.Any())
                return "No especificado";

            var horariosOrdenados = horarios.OrderBy(h => h.inicio).ToList();

            var grupos = new List<(TimeOnly inicio, TimeOnly fin)>();
            TimeOnly? grupoInicio = null;
            TimeOnly? grupoFin = null;

            foreach (var horario in horariosOrdenados)
            {
                if (grupoInicio == null)
                {
                    grupoInicio = horario.inicio;
                    grupoFin = horario.fin;
                }
                else if (grupoFin == horario.inicio)
                {
                    grupoFin = horario.fin;
                }
                else
                {
                    grupos.Add((grupoInicio.Value, grupoFin.Value));
                    grupoInicio = horario.inicio;
                    grupoFin = horario.fin;
                }
            }

            if (grupoInicio.HasValue && grupoFin.HasValue)
            {
                grupos.Add((grupoInicio.Value, grupoFin.Value));
            }

            var horariosFormateados = grupos.Select(g => $"{g.inicio:HH:mm} - {g.fin:HH:mm}");
            var totalHoras = horarios.Count;

            return $"{string.Join(", ", horariosFormateados)} ({totalHoras} {(totalHoras == 1 ? "hora" : "horas")})";
        }

        /// <summary>
        /// Une horarios consecutivos en rangos mayores.
        /// Ejemplo: [06:00-06:30, 06:30-07:00] → [06:00-07:00]
        /// </summary>
        private static List<HorarioDisponibleDto> UnirHorariosConsecutivos(List<HorarioDisponibleDto> horarios)
        {
            if (!horarios.Any())
                return horarios;

            var resultado = new List<HorarioDisponibleDto>();

            var actual = new HorarioDisponibleDto
            {
                IdHorarioCancha = horarios[0].IdHorarioCancha,
                HoraInicio = horarios[0].HoraInicio,
                HoraFin = horarios[0].HoraFin,
                Precio = horarios[0].Precio
            };

            for (int i = 1; i < horarios.Count; i++)
            {
                var siguiente = horarios[i];

                if (siguiente.HoraInicio == actual.HoraFin)
                {
                    actual.HoraFin = siguiente.HoraFin;
                    actual.Precio = (actual.Precio ?? 0) + (siguiente.Precio ?? 0);
                }
                else
                {
                    resultado.Add(actual);
                    actual = new HorarioDisponibleDto
                    {
                        IdHorarioCancha = siguiente.IdHorarioCancha,
                        HoraInicio = siguiente.HoraInicio,
                        HoraFin = siguiente.HoraFin,
                        Precio = siguiente.Precio
                    };
                }
            }

            resultado.Add(actual);
            return resultado;
        }
    }
}
