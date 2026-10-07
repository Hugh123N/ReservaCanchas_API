using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Operador;

public class OperadorDto
{
    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public int IdProveedor { get; set; }

}
