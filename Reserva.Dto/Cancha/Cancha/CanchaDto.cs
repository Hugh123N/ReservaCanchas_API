using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Cancha
{
    public class CanchaDto
    {
        public string Nombre { get; set; } = null!;
        public int IdTipoCancha { get; set; }
        public string? Descripcion { get; set; }
        public string? Ubicacion { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string? Direccion { get; set; }
        public decimal? PrecioHora { get; set; }
        public int? CreadoPor { get; set; }
        public int? IdProveedor { get; set; }
        public string? CodigoUbigeo { get; set; }
        public int IdEstadoCancha { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
