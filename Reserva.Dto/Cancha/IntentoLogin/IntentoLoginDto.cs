using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.IntentoLogin
{
    public class IntentoLoginDto
    {
        public Guid? IdUsuario { get; set; }
        public DateTimeOffset FechaIntento { get; set; }
        public bool Exitoso { get; set; }
    }
}
