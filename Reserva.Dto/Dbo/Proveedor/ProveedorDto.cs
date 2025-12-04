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
        public int IdEstadoProveedor { get; set; }
        public string? Telefono { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
    }
}
