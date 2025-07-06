using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.CanchaFavorita
{
    public class CanchaFavoritaDto
    {
        public int IdUsuario { get; set; }
        public int IdCancha { get; set; }
        public DateTimeOffset FechaAgregado { get; set; }
    }
}
