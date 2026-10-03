using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Proveedor
{
    public class ProveedorDto
    {
        public string? IdUsuario { get; set; }
        public string Nombres { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? Email { get; set; }
        public string? RazonSocial { get; set; }
        public string? Ruc { get; set; }
        public int IdTipoProveedor { get; set; }
        
    }
}
