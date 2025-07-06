using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Proveedor
{
    public class ProveedorDto
    {
        public string? RazonSocial { get; set; }
        public string? Ruc { get; set; }
        public string? Descripcion { get; set; }
        public int IdTipoProveedor { get; set; }
        public int IdEstadoProveedor { get; set; }
    }
}
