namespace Reserva.Entity;

public class Reclamacion
{
    public int IdReclamacion { get; set; }
    public string? NumeroHoja { get; set; }
    public DateTimeOffset FechaRegistro { get; set; }

    public string ComercioTitular { get; set; } = null!;
    public string ComercioNombre { get; set; } = null!;
    public string ComercioRuc { get; set; } = null!;
    public string ComercioDireccion { get; set; } = null!;
    public string ComercioTelefono { get; set; } = null!;
    public string ComercioEmail { get; set; } = null!;

    public string Nombres { get; set; } = null!;
    public string TipDoc { get; set; } = null!;
    public string NroDoc { get; set; } = null!;
    public string Domicilio { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public bool EsMenor { get; set; }
    public string? Representante { get; set; }

    public string DescripcionServicio { get; set; } = null!;
    public decimal? MontoReclamado { get; set; }
    public string? ReferenciaOperacion { get; set; }
    public string Tipo { get; set; } = null!;
    public string Detalle { get; set; } = null!;
    public string Pedido { get; set; } = null!;

    public string Estado { get; set; } = "PENDIENTE";
    public string? Observaciones { get; set; }
    public string? Respuesta { get; set; }
    public DateTimeOffset? FechaComunicacionRespuesta { get; set; }
    public string? ReferenciaRespuesta { get; set; }
    public string? ResponsableRespuesta { get; set; }

    public string? UserNameCreate { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public string? UserNameUpdate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    public bool Activo { get; set; }
}
