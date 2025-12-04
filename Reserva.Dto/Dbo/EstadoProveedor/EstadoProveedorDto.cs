using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.EstadoProveedor
{
    public class EstadoProveedorDto
    {
        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }
    }
}
