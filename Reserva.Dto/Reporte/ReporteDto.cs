namespace Reserva.Dto.Reporte;

public record ColumnaReporteDto(string Campo, string Titulo, string Formato = "texto");
public record IndicadorReporteDto(string Etiqueta, decimal Valor, string Formato = "numero");

public class ReporteDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Criterio { get; set; } = string.Empty;
    public DateOnly FechaDesde { get; set; }
    public DateOnly FechaHasta { get; set; }
    public DateTimeOffset GeneradoEn { get; set; }
    public string Cancha { get; set; } = "Todas las canchas autorizadas";
    public string EstadoReserva { get; set; } = "Todos";
    public string EstadoPago { get; set; } = "Todos";
    public List<ColumnaReporteDto> Columnas { get; set; } = new();
    public List<IndicadorReporteDto> Indicadores { get; set; } = new();
    public List<Dictionary<string, object?>> Filas { get; set; } = new();
}

public record OpcionReporteDto(string Codigo, string Nombre);
public record CanchaReporteDto(int IdCancha, string Nombre, string ZonaHoraria, bool Activo);

public class FiltrosReporteDto
{
    public DateOnly Hoy { get; set; }
    public List<CanchaReporteDto> Canchas { get; set; } = new();
    public List<OpcionReporteDto> EstadosReserva { get; set; } = new();
    public List<OpcionReporteDto> EstadosPago { get; set; } = new();
}

public record ArchivoReporteDto(byte[] Contenido, string Nombre);
