using System;
using System.Collections.Generic;

namespace Reserva.Entity;

public partial class Cancha
{
    public int IdCancha { get; set; }

    public int IdProveedor { get; set; }

    public int IdTipoSuperficie { get; set; }

    public int IdEstadoCancha { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public string? TelefonoCancha { get; set; }

    public string? Direccion { get; set; }

    public string? CodigoUbigeo { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public int? CapacidadJugadores { get; set; }

    public bool TieneTecho { get; set; }

    public bool TieneIluminacion { get; set; }

    public string? Pais { get; set; }

    public string ZonaHoraria { get; set; } = null!;

    public string UserNameCreate { get; set; } = null!;

    public DateTimeOffset CreateDate { get; set; }

    public string? UserNameUpdate { get; set; }

    public DateTimeOffset? UpdateDate { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<BloqueoHorario> BloqueoHorario { get; set; } = new List<BloqueoHorario>();

    public virtual ICollection<CanchaFavorita> CanchaFavorita { get; set; } = new List<CanchaFavorita>();

    public virtual Ubigeo? CodigoUbigeoNavigation { get; set; }

    public virtual ICollection<HorarioCancha> HorarioCancha { get; set; } = new List<HorarioCancha>();

    public virtual EstadoCancha IdEstadoCanchaNavigation { get; set; } = null!;

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual TipoSuperficie IdTipoSuperficieNavigation { get; set; } = null!;

    public virtual ICollection<ImagenCancha> ImagenCancha { get; set; } = new List<ImagenCancha>();

    public virtual ICollection<OperadorCancha> OperadorCancha { get; set; } = new List<OperadorCancha>();

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();

    public virtual ICollection<ServicioCancha> ServicioCancha { get; set; } = new List<ServicioCancha>();

    public virtual ICollection<TipoDeporteCancha> TipoDeporteCancha { get; set; } = new List<TipoDeporteCancha>();
}
