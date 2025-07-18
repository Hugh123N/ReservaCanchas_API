﻿using System;
using System.Collections.Generic;

namespace Reserva.Entity.Models;

public partial class Cancha
{
    public int IdCancha { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdTipoCancha { get; set; }

    public string? Descripcion { get; set; }

    public string? Ubicacion { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? Direccion { get; set; }

    public decimal? PrecioHora { get; set; }

    public int? CreadoPor { get; set; }

    public int? IdProveedor { get; set; }

    public string? CodigoUbigeo { get; set; }

    public int IdEstadoCancha { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public bool Activo { get; set; }

    public virtual Ubigeo? CodigoUbigeoNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<Disponibilidad> Disponibilidads { get; set; } = new List<Disponibilidad>();

    public virtual EstadoCancha IdEstadoCanchaNavigation { get; set; } = null!;

    public virtual Proveedor? IdProveedorNavigation { get; set; }

    public virtual TipoCancha IdTipoCanchaNavigation { get; set; } = null!;

    public virtual ICollection<ImagenCancha> ImagenCanchas { get; set; } = new List<ImagenCancha>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
