using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Notificacion
{
    public class NotificacionDto
    {
        public int IdUsuario { get; set; }
        public string Mensaje { get; set; } = null!;
        public bool? Leido { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
