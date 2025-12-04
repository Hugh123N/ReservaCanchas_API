using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Notificacion
{
    public class NotificacionDto
    {
        public Guid IdUsuario { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public string? Tipo { get; set; }
        public bool? Leido { get; set; }
        public int? IdReserva { get; set; }
        public DateTimeOffset? FechaCreacion { get; set; }
        public DateTimeOffset? FechaLeido { get; set; }
    }
}
