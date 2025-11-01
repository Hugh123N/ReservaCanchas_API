using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class AspNetUserTokens
{
    public Guid UserId { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }
}
