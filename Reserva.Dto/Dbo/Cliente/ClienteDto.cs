using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Cliente;

public class ClienteDto
{
    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? UserId { get; set; }

}
