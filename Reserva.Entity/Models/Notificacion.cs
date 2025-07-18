﻿using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public int IdUsuario { get; set; }

    public string Mensaje { get; set; } = null!;

    public bool? Leido { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool Activo { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
