using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.IntentoLogin
{
    public class IntentoLoginDto
    {
        public Guid? IdUsuario { get; set; }
        public DateTimeOffset FechaIntento { get; set; }
        public bool Exitoso { get; set; }
    }
}
