using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.ImagenCancha
{
    public class ImagenCanchaDto
    {
        public int IdCancha { get; set; }
        public string UrlImagen { get; set; } = null!;
        public bool? EsPrincipal { get; set; }
    }
}
