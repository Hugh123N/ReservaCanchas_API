using Reserva.Dto.Reporte;

namespace Reserva.Domain.Services.Reporte;

public static class ControlMovimientosBuilder
{
    public static string Responsable(MovimientoReporteData m) => !m.IdUsuarioResponsable.HasValue || m.Tipo == "SALDO_INICIAL"
        ? "Sin identificación histórica" : !string.IsNullOrWhiteSpace(m.NombreResponsable) ? m.NombreResponsable :
        !string.IsNullOrWhiteSpace(m.NombreAuditoria) ? m.NombreAuditoria : $"Usuario {m.IdUsuarioResponsable}";

    public static ControlMovimientosDto Construir(List<MovimientoReporteData> datos, List<CanchaReporteDto> canchas,
        FiltroMovimientosReporteDto filtro)
    {
        var nombres = canchas.ToDictionary(c => c.IdCancha, c => c.Nombre);
        var acciones = datos.Where(m => m.Tipo is "COBRO" or "REEMBOLSO" or "CORRECCION").ToList();
        var dinero = acciones.Where(m => m.Tipo is "COBRO" or "REEMBOLSO").ToList();
        var correcciones = acciones.Where(m => m.Tipo == "CORRECCION").ToList();
        var historicos = datos.Where(m => m.Tipo == "SALDO_INICIAL").ToList();
        ReporteDto Tabla(string titulo, string criterio) => new()
        {
            Titulo = titulo, Criterio = criterio, FechaDesde = filtro.FechaDesde, FechaHasta = filtro.FechaHasta,
            GeneradoEn = DateTimeOffset.UtcNow, Cancha = filtro.IdCancha.HasValue ? nombres[filtro.IdCancha.Value] : "Todas las canchas del negocio"
        };
        var response = new ControlMovimientosDto
        {
            ResponsableFiltro = filtro.SoloSinResponsable ? "Sin identificación histórica" : filtro.IdUsuarioResponsable.HasValue ?
                datos.FirstOrDefault(m => m.IdUsuarioResponsable == filtro.IdUsuarioResponsable && m.Tipo != "SALDO_INICIAL") is { } actor ? Responsable(actor) : "Responsable seleccionado, sin movimientos" : "Todos los responsables",
            TipoFiltro = string.IsNullOrEmpty(filtro.Tipo) ? "Todos" : filtro.Tipo,
            CodigoReservaFiltro = string.IsNullOrWhiteSpace(filtro.CodigoReserva) ? null : filtro.CodigoReserva.Trim(),
            Responsables = Tabla("Resumen por responsable y cancha", "Por fecha local de operación. Neto = cobros menos reembolsos. Las correcciones y saldos históricos no son entradas/salidas nuevas."),
            Metodos = Tabla("Desglose por método y cancha", "Solo cobros y reembolsos. El método corresponde a cada operación, no a la cabecera de pago."),
            Movimientos = Tabla("Detalle de cobros y reembolsos", "Cada fila es una operación. Incluye la fecha de la reserva aunque se juegue otro día."),
            Correcciones = Tabla("Correcciones de registro", "Ajustes globales realizados por el responsable indicado. No son cobros, devoluciones ni prueba de quién cometió el error original. El filtro de método no se aplica a estos ajustes."),
            Historicos = Tabla("Saldos históricos incorporados", "La fecha es de incorporación al historial, no del cobro original. No se atribuyen al dinero ni al responsable del período."),
            DetalleExcel = Tabla("Todas las operaciones", "Cobros, reembolsos, ajustes e históricos en columnas separadas. Anterior/resultante corresponden a lo cobrado, salvo REEMBOLSO donde corresponden a lo devuelto. Los históricos no son cobros del período.")
        };
        var indicadores = new List<IndicadorReporteDto>
        {
            new("Cobrado registrado", dinero.Where(m => m.Tipo == "COBRO").Sum(m => m.Importe), "moneda"),
            new("Reembolsado", dinero.Where(m => m.Tipo == "REEMBOLSO").Sum(m => m.Importe), "moneda"),
            new("Neto de movimientos", dinero.Sum(m => m.Tipo == "COBRO" ? m.Importe : -m.Importe), "moneda"),
            new("Reservas con movimientos", acciones.Select(m => m.IdReserva).Distinct().Count())
        };
        response.Responsables.Indicadores = indicadores;
        response.Correcciones.Indicadores = new() { new("Correcciones realizadas", correcciones.Count), new("Ajuste registrado", correcciones.Sum(m => m.Importe), "moneda") };
        response.Responsables.Columnas = new()
        {
            new("responsable", "Responsable"), new("rol", "Rol en la operación"), new("cancha", "Cancha"),
            new("cobros", "Cobros", "moneda"), new("reembolsos", "Reembolsos", "moneda"), new("neto", "Neto", "moneda"),
            new("operaciones", "Cobros/devoluciones", "numero"), new("reservas", "Reservas distintas", "numero"),
            new("correcciones", "Correcciones realizadas", "numero"), new("ajuste", "Ajuste de registro", "moneda")
        };
        foreach (var g in acciones.GroupBy(m => (m.IdUsuarioResponsable, m.RolResponsable, m.IdCancha)))
        {
            var cobros = g.Where(m => m.Tipo == "COBRO").Sum(m => m.Importe);
            var reembolsos = g.Where(m => m.Tipo == "REEMBOLSO").Sum(m => m.Importe);
            response.Responsables.Filas.Add(new()
            {
                ["responsable"] = Responsable(g.First()),
                ["rol"] = g.Key.RolResponsable ?? "Rol no registrado", ["cancha"] = nombres[g.Key.IdCancha],
                ["cobros"] = cobros, ["reembolsos"] = reembolsos, ["neto"] = cobros - reembolsos,
                ["operaciones"] = g.Count(m => m.Tipo is "COBRO" or "REEMBOLSO"), ["reservas"] = g.Select(m => m.IdReserva).Distinct().Count(),
                ["correcciones"] = g.Count(m => m.Tipo == "CORRECCION"), ["ajuste"] = g.Where(m => m.Tipo == "CORRECCION").Sum(m => m.Importe)
            });
        }
        response.Responsables.Filas = response.Responsables.Filas.OrderBy(f => f["responsable"]).ThenBy(f => f["cancha"]).ToList();
        response.Metodos.Columnas = new() { new("responsable", "Responsable"), new("rol", "Rol"), new("cancha", "Cancha"),
            new("metodo", "Método"), new("cobros", "Cobros", "moneda"), new("reembolsos", "Reembolsos", "moneda"), new("neto", "Neto", "moneda") };
        foreach (var g in dinero.GroupBy(m => (m.IdUsuarioResponsable, m.RolResponsable, m.IdCancha, m.IdMetodoPago)))
        {
            var cobros = g.Where(m => m.Tipo == "COBRO").Sum(m => m.Importe);
            var reembolsos = g.Where(m => m.Tipo == "REEMBOLSO").Sum(m => m.Importe);
            response.Metodos.Filas.Add(new() { ["responsable"] = Responsable(g.First()), ["rol"] = g.Key.RolResponsable ?? "Rol no registrado",
                ["cancha"] = nombres[g.Key.IdCancha], ["metodo"] = g.First().Metodo ?? "Sin método registrado", ["cobros"] = cobros,
                ["reembolsos"] = reembolsos, ["neto"] = cobros - reembolsos });
        }
        var baseColumnas = new List<ColumnaReporteDto>
        {
            new("fecha", "Fecha/hora local", "fechaHora"), new("responsable", "Responsable"), new("rol", "Rol en la operación"),
            new("cancha", "Cancha"), new("codigo", "Reserva"), new("fechaReserva", "Fecha de reserva", "fecha")
        };
        response.Movimientos.Columnas = new(baseColumnas) { new("tipo", "Acción"), new("importe", "Importe", "moneda"), new("metodo", "Método"), new("detalle", "Referencia y motivo", "detalle") };
        response.Correcciones.Columnas = new(baseColumnas) { new("importe", "Ajuste firmado", "moneda"), new("anterior", "Cobrado anterior", "moneda"), new("resultante", "Cobrado correcto", "moneda"), new("detalle", "Motivo", "detalle") };
        response.Historicos.Columnas = new(baseColumnas) { new("importe", "Saldo incorporado", "moneda"), new("detalle", "Observación", "detalle") };
        response.Historicos.Columnas[0] = new("fecha", "Incorporación local", "fechaHora");
        response.DetalleExcel.Columnas = new(baseColumnas) { new("tipo", "Tipo"), new("metodo", "Método"),
            new("entrada", "Entrada monetaria", "moneda"), new("salida", "Salida monetaria", "moneda"), new("ajuste", "Ajuste de registro", "moneda"),
            new("historico", "Saldo histórico", "moneda"), new("anterior", "Acumulado anterior", "moneda"), new("resultante", "Acumulado resultante", "moneda"),
            new("referencia", "Referencia"), new("motivo", "Motivo") };
        foreach (var m in datos.OrderByDescending(m => m.FechaUtc).ThenByDescending(m => m.IdMovimiento))
        {
            var fila = new Dictionary<string, object?>
            {
                ["fecha"] = m.FechaLocal, ["responsable"] = Responsable(m),
                ["rol"] = m.Tipo == "SALDO_INICIAL" ? "No atribuible" : m.RolResponsable ?? "Rol no registrado",
                ["cancha"] = nombres[m.IdCancha], ["codigo"] = m.CodigoReserva, ["idReserva"] = m.IdReserva, ["fechaReserva"] = m.FechaReservaLocal,
                ["tipo"] = m.Tipo, ["importe"] = m.Importe, ["metodo"] = m.Metodo ?? "Sin método registrado",
                ["anterior"] = m.Anterior, ["resultante"] = m.Resultante, ["referencia"] = m.Referencia, ["motivo"] = m.Motivo,
                ["detalle"] = string.Join("\n", new[] { m.Referencia == null ? null : $"Referencia: {m.Referencia}", m.Motivo }.Where(s => !string.IsNullOrEmpty(s))),
                ["entrada"] = m.Tipo == "COBRO" ? m.Importe : 0m, ["salida"] = m.Tipo == "REEMBOLSO" ? m.Importe : 0m,
                ["ajuste"] = m.Tipo == "CORRECCION" ? m.Importe : 0m, ["historico"] = m.Tipo == "SALDO_INICIAL" ? m.Importe : 0m
            };
            if (m.Tipo is "COBRO" or "REEMBOLSO") response.Movimientos.Filas.Add(fila);
            else if (m.Tipo == "CORRECCION") response.Correcciones.Filas.Add(fila);
            else if (m.Tipo == "SALDO_INICIAL") response.Historicos.Filas.Add(fila);
            response.DetalleExcel.Filas.Add(fila);
        }
        foreach (var tabla in new[] { response.Responsables, response.Metodos, response.Movimientos, response.Correcciones, response.Historicos, response.DetalleExcel })
        {
            var campos = tabla.Columnas.Select(c => c.Campo).Append("idReserva").ToHashSet();
            tabla.Filas = tabla.Filas.Select(f => f.Where(c => campos.Contains(c.Key)).ToDictionary(c => c.Key, c => c.Value)).ToList();
        }
        return response;
    }
}
