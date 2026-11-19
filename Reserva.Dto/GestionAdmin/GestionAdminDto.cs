namespace Reserva.Dto.GestionAdmin;

public class FiltroGestionAdminDto
{
    public string? Texto { get; set; }
    public string? Estado { get; set; }
    public int? IdProveedor { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 20;
}

public class PaginaGestionAdminDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
}

public class CambiarEstadoGestionDto
{
    public string Estado { get; set; } = "";
    public DateTimeOffset Version { get; set; }
}

public class ProveedorAdminDto
{
    public int IdProveedor { get; set; }
    public string Nombre { get; set; } = "";
    public string Titular { get; set; } = "";
    public string? Documento { get; set; }
    public string? Ruc { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string Estado { get; set; } = "";
    public DateTimeOffset Registro { get; set; }
    public DateTimeOffset Version { get; set; }
    public int Canchas { get; set; }
    public int ReservasFuturas { get; set; }
    public string? Plan { get; set; }
    public string? EstadoPlan { get; set; }
    public DateTimeOffset? FinPlan { get; set; }
    public bool EsPrueba { get; set; }
    public bool AutoRenovacion { get; set; }
}

public class CanchaAdminDto
{
    public int IdCancha { get; set; }
    public int IdProveedor { get; set; }
    public string Nombre { get; set; } = "";
    public string Proveedor { get; set; } = "";
    public string? Direccion { get; set; }
    public string? Imagen { get; set; }
    public string Estado { get; set; } = "";
    public bool Publicada { get; set; }
    public bool ProveedorHabilitado { get; set; }
    public DateTimeOffset Registro { get; set; }
    public DateTimeOffset Version { get; set; }
}

public class EstadoNegocioDto
{
    public string Estado { get; set; } = "";
    public bool PuedeOperar { get; set; }
    public string Mensaje { get; set; } = "";
}
