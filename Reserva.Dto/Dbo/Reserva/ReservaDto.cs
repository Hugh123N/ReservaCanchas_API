using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Reserva
{
    public class ReservaDto
    {
        public Guid IdUsuario { get; set; }
        public int IdCancha { get; set; }
        public DateTimeOffset Fecha { get; set; }
        public decimal? Monto { get; set; }
        public int IdEstadoReserva { get; set; }

    }
}
