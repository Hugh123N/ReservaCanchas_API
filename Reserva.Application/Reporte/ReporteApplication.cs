using MediatR;
using Reserva.Application.Abstractions.Reporte;
using Reserva.Application.Base;
using Reserva.Common.Excel;
using Reserva.Domain.Queries.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Application.Reporte;

public class ReporteApplication(IMediator mediator) : ApplicationBase(mediator), IReporteApplication
{
    public Task<ResponseDto<FiltrosReporteDto>> Filtros(CancellationToken cancellationToken)
        => _mediator.Send(new GetFiltrosReporteQuery(), cancellationToken);

    public Task<ResponseDto<ReporteDto>> Consultar(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken)
        => _mediator.Send(new GetReporteQuery(tipo, filtro), cancellationToken);

    public async Task<ResponseDto<ArchivoReporteDto>> Exportar(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken)
    {
        var resultado = await Consultar(tipo, filtro, cancellationToken);
        var response = new ResponseDto<ArchivoReporteDto>();
        if (!resultado.IsValid || resultado.Data == null)
        {
            response.AttachResults(resultado);
            if (resultado.IsValid) response.AddErrorResult("No se pudo generar el reporte.");
            return response;
        }
        var reporte = resultado.Data;
        var resumen = new List<(string, object?, string)>
        {
            ("Desde", reporte.FechaDesde, "fecha"), ("Hasta", reporte.FechaHasta, "fecha"),
            ("Cancha", reporte.Cancha, "texto"), ("Estado de reserva", reporte.EstadoReserva, "texto"),
            ("Estado de pago", reporte.EstadoPago, "texto"), ("Generado (UTC)", reporte.GeneradoEn.UtcDateTime, "fechaHora"),
            ("Criterio", reporte.Criterio, "texto")
        };
        resumen.AddRange(reporte.Indicadores.Select(i => (i.Etiqueta, (object?)i.Valor, i.Formato)));
        var columnas = reporte.Columnas.Select(c => new ColumnaExcel(c.Titulo, c.Formato)).ToList();
        var filas = reporte.Filas.Select(f => (IReadOnlyList<object?>)reporte.Columnas.Select(c => f.GetValueOrDefault(c.Campo)).ToList());
        var contenido = ExportadorExcel.Generar(reporte.Titulo, columnas, filas, resumen);
        var nombre = $"reporte-{tipo.ToString().ToLowerInvariant()}-{filtro.FechaDesde:yyyyMMdd}-{filtro.FechaHasta:yyyyMMdd}.xlsx";
        response.UpdateData(new ArchivoReporteDto(contenido, nombre));
        return response;
    }
}
