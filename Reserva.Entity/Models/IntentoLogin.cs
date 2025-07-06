using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class IntentoLogin
{
    public long IdIntentoLogin { get; set; }

    public int? IdUsuario { get; set; }

    public DateTimeOffset FechaIntento { get; set; }

    public bool Exitoso { get; set; }

    public bool Activo { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
