using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Cancha
{
    public class CanchaDto
    {
        public int IdProveedor { get; set; }

        public int IdTipoSuperficie { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public string? TelefonoCancha { get; set; }

        public string? Direccion { get; set; }

        public string? CodigoUbigeo { get; set; }

        public decimal? Latitud { get; set; }

        public decimal? Longitud { get; set; }

        public int? CapacidadJugadores { get; set; }

        public bool TieneTecho { get; set; }

        public bool TieneIluminacion { get; set; }

        
    }
}
