using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Reserva
{
    public class ReservaDto
    {
        public int IdUsuario { get; set; }
        public int IdCancha { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int IdEstadoReserva { get; set; }
        public int? CreadoPor { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
