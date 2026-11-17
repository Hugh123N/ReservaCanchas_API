namespace Reserva.Dto.Reporte;

public class FiltroMovimientosReporteDto
{
    public DateOnly FechaDesde { get; set; }
    public DateOnly FechaHasta { get; set; }
    public int? IdCancha { get; set; }
    public Guid? IdUsuarioResponsable { get; set; }
    public bool SoloSinResponsable { get; set; }
    public int? IdMetodoPago { get; set; }
    public string? Tipo { get; set; }
    public string? CodigoReserva { get; set; }
}

public class FiltroControlMovimientosDto
{
    public FiltroMovimientosReporteDto Movimientos { get; set; } = new();
}

public record ResponsableReporteDto(Guid IdUsuario, string Nombre, bool Activo);
public record MetodoReporteDto(int IdMetodoPago, string Nombre);

public class FiltrosControlMovimientosDto
{
    public DateOnly Hoy { get; set; }
    public List<CanchaReporteDto> Canchas { get; set; } = new();
    public List<ResponsableReporteDto> Responsables { get; set; } = new();
    public List<MetodoReporteDto> Metodos { get; set; } = new();
}

public class ControlMovimientosDto
{
    public string ResponsableFiltro { get; set; } = "Todos los responsables";
    public string MetodoFiltro { get; set; } = "Todos los métodos";
    public string TipoFiltro { get; set; } = "Todos";
    public string? CodigoReservaFiltro { get; set; }
    public ReporteDto Responsables { get; set; } = new();
    public ReporteDto Metodos { get; set; } = new();
    public ReporteDto Movimientos { get; set; } = new();
    public ReporteDto Correcciones { get; set; } = new();
    public ReporteDto Historicos { get; set; } = new();
    [System.Text.Json.Serialization.JsonIgnore]
    public ReporteDto DetalleExcel { get; set; } = new();
}
