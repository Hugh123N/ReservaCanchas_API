using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Proveedor
{
    public class ProveedorDto
    {
        public Guid? IdUsuario { get; set; }
        public string? RazonSocial { get; set; }
        public string? Ruc { get; set; }
        public int IdTipoProveedor { get; set; }
        
    }
}
