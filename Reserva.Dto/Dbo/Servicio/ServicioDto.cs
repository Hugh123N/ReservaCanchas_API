using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Servicio;

public class ServicioDto
{

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Icono { get; set; }


}
