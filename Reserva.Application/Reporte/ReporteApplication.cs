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
    public Task<ResponseDto<FiltrosControlMovimientosDto>> FiltrosControl(CancellationToken cancellationToken)
        => _mediator.Send(new GetFiltrosControlMovimientosQuery(), cancellationToken);

    public Task<ResponseDto<ControlMovimientosDto>> Control(FiltroControlMovimientosDto filtro, CancellationToken cancellationToken)
        => _mediator.Send(new GetControlMovimientosQuery(filtro), cancellationToken);

    public async Task<ResponseDto<ArchivoReporteDto>> ExportarControl(FiltroControlMovimientosDto filtro, CancellationToken cancellationToken)
    {
        var resultado = await Control(filtro, cancellationToken);
        var response = new ResponseDto<ArchivoReporteDto>();
        if (!resultado.IsValid || resultado.Data == null)
        {
            response.AttachResults(resultado);
            if (resultado.IsValid) response.AddErrorResult("No se pudo generar el control de movimientos.");
            return response;
        }
        var data = resultado.Data;
        var resumen = new List<(string, object?, string)>
        {
            ("Movimientos desde", filtro.Movimientos.FechaDesde, "fecha"), ("Movimientos hasta", filtro.Movimientos.FechaHasta, "fecha"),
            ("Canchas de movimientos", data.Responsables.Cancha, "texto"),
            ("Responsable", data.ResponsableFiltro, "texto"),
            ("Método", data.MetodoFiltro, "texto"), ("Tipo", data.TipoFiltro, "texto"), ("Código buscado", data.CodigoReservaFiltro, "texto"),
            ("Generado (UTC)", data.Responsables.GeneradoEn.UtcDateTime, "fechaHora"),
            ("Criterio financiero", data.Responsables.Criterio, "texto"), ("Correcciones", data.Correcciones.Criterio, "texto"),
            ("Históricos", data.Historicos.Criterio, "texto"),
            ("Base de acumulados", data.DetalleExcel.Criterio, "texto")
        };
        resumen.AddRange(data.Responsables.Indicadores.Select(i => ($"Movimientos: {i.Etiqueta}", (object?)i.Valor, i.Formato)));
        resumen.AddRange(data.Correcciones.Indicadores.Select(i => ($"Correcciones: {i.Etiqueta}", (object?)i.Valor, i.Formato)));
        var hojas = new[] { Hoja("Responsables", data.Responsables), Hoja("Metodos", data.Metodos), Hoja("Movimientos", data.DetalleExcel) };
        cancellationToken.ThrowIfCancellationRequested();
        var bytes = ExportadorExcel.GenerarLibro("Control de movimientos y responsables", resumen, hojas);
        response.UpdateData(new ArchivoReporteDto(bytes, $"control-movimientos-{filtro.Movimientos.FechaDesde:yyyyMMdd}-{filtro.Movimientos.FechaHasta:yyyyMMdd}.xlsx"));
        return response;
    }

    private static HojaExcel Hoja(string nombre, ReporteDto reporte) => new(nombre, reporte.Titulo,
        reporte.Columnas.Select(c => new ColumnaExcel(c.Titulo, c.Formato)).ToList(),
        reporte.Filas.Select(f => (IReadOnlyList<object?>)reporte.Columnas.Select(c => f.GetValueOrDefault(c.Campo)).ToList()));
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
        var contenido = ExportadorExcel.GenerarLibro(reporte.Titulo, resumen, new[] { Hoja("Detalle", reporte) });
        var nombre = $"reporte-{tipo.ToString().ToLowerInvariant()}-{filtro.FechaDesde:yyyyMMdd}-{filtro.FechaHasta:yyyyMMdd}.xlsx";
        response.UpdateData(new ArchivoReporteDto(contenido, nombre));
        return response;
    }
}
