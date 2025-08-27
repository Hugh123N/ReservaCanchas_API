using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class AspNetUserLogins
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public Guid UserId { get; set; }

    public string? ProviderDisplayName { get; set; }

    public virtual AspNetUsers User { get; set; } = null!;
}
