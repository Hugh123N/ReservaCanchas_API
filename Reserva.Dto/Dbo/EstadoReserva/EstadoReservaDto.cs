using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.EstadoReserva
{
    public class EstadoReservaDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
