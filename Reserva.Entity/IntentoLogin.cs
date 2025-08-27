using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class IntentoLogin
{
    public long IdIntentoLogin { get; set; }

    public Guid? IdUsuario { get; set; }

    public DateTimeOffset FechaIntento { get; set; }

    public bool Exitoso { get; set; }

    public bool Activo { get; set; }

    public virtual AspNetUsers? IdUsuarioNavigation { get; set; }
}
