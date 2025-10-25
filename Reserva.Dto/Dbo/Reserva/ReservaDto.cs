using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Reserva
{
    public class ReservaDto
    {
        public Guid IdUsuario { get; set; }
        public int IdCancha { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int IdEstadoReserva { get; set; }
    }
}
