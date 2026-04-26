using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Plane;

public class PlaneDto
{
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int? OrdenVisual { get; set; }
}
