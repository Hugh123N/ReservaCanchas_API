using System.Text.Json;
using Reserva.Common;
using Reserva.Domain.Queries.Reporte;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Reporte;
using Reserva.Domain.Queries.Dbo.Reserva;
using Reserva.Dto.Dbo.Reserva;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class ControlMovimientosTests
{
    private static readonly Guid Luis = Guid.Parse("c876137f-c342-41f4-863d-59ac05506763");
    private static readonly Guid Dueno = Guid.Parse("ee16ce98-f1dc-4b5f-bbb5-c30750422f14");
    private static readonly List<CanchaReporteDto> Canchas = new() { new(3, "Cancha Norte", "America/Lima", true), new(4, "Cancha Sur", "America/Lima", true) };

    [Fact]
    public void Neto_ExcluyeCorreccionesEHistoricosYSeparaCadaCancha()
    {
        var datos = new List<MovimientoReporteData> { Movimiento("COBRO", 100, Luis, 3), Movimiento("REEMBOLSO", 20, Luis, 3),
            Movimiento("COBRO", 50, Luis, 4), Movimiento("CORRECCION", -10, Dueno, 3), Movimiento("SALDO_INICIAL", 500, null, 3) };
        var reporte = ControlMovimientosBuilder.Construir(datos, Canchas, Filtro());
        Assert.Equal(150, reporte.Responsables.Indicadores.Single(i => i.Etiqueta == "Cobrado registrado").Valor);
        Assert.Equal(20, reporte.Responsables.Indicadores.Single(i => i.Etiqueta == "Reembolsado").Valor);
        Assert.Equal(130, reporte.Responsables.Indicadores.Single(i => i.Etiqueta == "Neto de movimientos").Valor);
        Assert.Equal(-10, reporte.Correcciones.Indicadores.Single(i => i.Etiqueta == "Ajuste registrado").Valor);
        Assert.Equal(3, reporte.Movimientos.Filas.Count);
        Assert.Single(reporte.Historicos.Filas);
        Assert.Equal("Sin identificación histórica", reporte.Historicos.Filas[0]["responsable"]);
        var porLuis = reporte.Responsables.Filas.Where(f => (string?)f["responsable"] == "Luis").ToList();
        Assert.Equal(2, porLuis.Count);
        Assert.Equal(new[] { "Cancha Norte", "Cancha Sur" }, porLuis.Select(f => f["cancha"]));
        Assert.All(reporte.DetalleExcel.Filas, fila => Assert.Contains(fila["cancha"], new object?[] { "Cancha Norte", "Cancha Sur" }));
    }

    [Fact]
    public void Responsables_SeIdentificanPorGuidAunqueTenganElMismoNombre()
    {
        var datos = new List<MovimientoReporteData> { Movimiento("COBRO", 50, Luis, 3), Movimiento("COBRO", 80, Dueno, 3) };
        datos.ForEach(m => { m.NombreResponsable = "Mismo nombre"; m.RolResponsable = Constants.Role.Operador; });
        var reporte = ControlMovimientosBuilder.Construir(datos, Canchas, Filtro());
        Assert.Equal(2, reporte.Responsables.Filas.Count);
        Assert.Equal(new[] { 50m, 80m }, reporte.Responsables.Filas.Select(f => (decimal)f["cobros"]!).OrderBy(v => v));
        foreach (var tabla in new[] { reporte.Responsables, reporte.Metodos, reporte.Movimientos, reporte.Correcciones, reporte.Historicos, reporte.DetalleExcel })
        {
            Assert.DoesNotContain(tabla.Columnas, c => c.Campo == "identidad");
            Assert.All(tabla.Filas, fila => Assert.False(fila.ContainsKey("identidad")));
        }
    }

    [Fact]
    public void ReembolsoDeDineroAntiguo_PuedeDarNetoNegativoSinOcultarlo()
    {
        var datos = new List<MovimientoReporteData> { Movimiento("REEMBOLSO", 70, Dueno, 4) };
        var reporte = ControlMovimientosBuilder.Construir(datos, Canchas, Filtro());
        Assert.Equal(-70, reporte.Responsables.Indicadores.Single(i => i.Etiqueta == "Neto de movimientos").Valor);
        Assert.Equal(70m, reporte.DetalleExcel.Filas[0]["salida"]);
        Assert.Equal(0m, reporte.DetalleExcel.Filas[0]["entrada"]);
    }

    [Fact]
    public void Abonos_NoDuplicanLaCantidadGlobalDeReservas()
    {
        var datos = new List<MovimientoReporteData> { Movimiento("COBRO", 30, Luis, 3), Movimiento("COBRO", 20, Dueno, 3) };
        var reporte = ControlMovimientosBuilder.Construir(datos, Canchas, Filtro());
        Assert.Equal(1, reporte.Responsables.Indicadores.Single(i => i.Etiqueta == "Reservas con movimientos").Valor);
    }

    [Fact]
    public void DatosParaExcel_SeparanDineroAjusteYSaldoAnteriorYNoDuplicanLaRespuestaJson()
    {
        var datos = new List<MovimientoReporteData> { Movimiento("CORRECCION", 10, Dueno, 3), Movimiento("SALDO_INICIAL", 900, null, 3) };
        var reporte = ControlMovimientosBuilder.Construir(datos, Canchas, Filtro());
        Assert.Equal(10m, reporte.DetalleExcel.Filas.Single(f => (string)f["tipo"]! == "CORRECCION")["ajuste"]);
        Assert.Equal(900m, reporte.DetalleExcel.Filas.Single(f => (string)f["tipo"]! == "SALDO_INICIAL")["historico"]);
        Assert.DoesNotContain("DetalleExcel", JsonSerializer.Serialize(reporte));
    }

    [Fact]
    public void Periodos_UsanFechaLocalDeOperacionAunqueLaReservaSeaOtroDia()
    {
        var (desde, hasta) = ReportePeriodo.LimitesUtc(Canchas, new(2026, 11, 13), new(2026, 11, 13));
        Assert.Equal(new DateTimeOffset(2026, 11, 13, 5, 0, 0, TimeSpan.Zero), desde);
        Assert.Equal(new DateTimeOffset(2026, 11, 14, 5, 0, 0, TimeSpan.Zero), hasta);
        var datos = new List<MovimientoReporteData> { Movimiento("COBRO", 50, Luis, 3) };
        datos[0].FechaReservaLocal = new(2026, 11, 20);
        var reporte = ControlMovimientosBuilder.Construir(datos, Canchas, Filtro());
        Assert.Equal(new DateOnly(2026, 11, 20), reporte.Movimientos.Filas[0]["fechaReserva"]);
        Assert.Equal(new DateTime(2026, 11, 13, 10, 0, 0), reporte.Movimientos.Filas[0]["fecha"]);
    }

    [Fact]
    public void Search_ConservaAnuladasComoSubconjuntoDeCanceladasEnElResumen()
    {
        var pendientes = new ReservaClienteDto { IdReserva = 1, CodigoEstadoReserva = Constants.ESTADO_RESERVA.Pendiente, MontoPendiente = 100 };
        var anulada = new ReservaClienteDto { IdReserva = 2, Anulada = true, CodigoEstadoReserva = Constants.ESTADO_RESERVA.Cancelado };
        var resumen = SearchReservaConsultas.Indicadores(new[] { pendientes, anulada }.AsQueryable()).Single();
        Assert.Equal(1, resumen.Pendientes);
        Assert.Equal(1, resumen.Canceladas);
        Assert.Equal(1, resumen.Anuladas);
        Assert.Equal(100, resumen.SaldoVigente);
    }

    [Fact]
    public async Task Validador_RevisaElPeriodoDeMovimientosYNoAceptaIdentificacionContradictoria()
    {
        var validator = new GetControlMovimientosQueryValidator(new GetReporteQueryValidator());
        var filtro = new FiltroControlMovimientosDto { Movimientos = Filtro() };
        Assert.True((await validator.ValidateAsync(new GetControlMovimientosQuery(filtro))).IsValid);
        filtro.Movimientos.SoloSinResponsable = true;
        filtro.Movimientos.IdUsuarioResponsable = Luis;
        Assert.False((await validator.ValidateAsync(new GetControlMovimientosQuery(filtro))).IsValid);
        filtro.Movimientos.SoloSinResponsable = false;
        filtro.Movimientos.Tipo = "CORRECCION";
        filtro.Movimientos.IdMetodoPago = 2;
        Assert.False((await validator.ValidateAsync(new GetControlMovimientosQuery(filtro))).IsValid);
        filtro.Movimientos.IdMetodoPago = null;
        filtro.Movimientos.FechaHasta = new(2026, 11, 1);
        Assert.False((await validator.ValidateAsync(new GetControlMovimientosQuery(filtro))).IsValid);
    }

    [Fact]
    public void UltimoReembolso_UsaSuFechaYResponsableRealSinInventarDatosHistoricos()
    {
        var filas = new List<ReservaClienteDto>
        {
            new() { IdReserva = 10, IdCancha = 3, MontoReembolso = 40, Fecha = new(2026, 11, 13, 15, 0, 0, TimeSpan.Zero) },
            new() { IdReserva = 11, IdCancha = 4, MontoReembolso = 20, Fecha = new(2026, 11, 13, 15, 0, 0, TimeSpan.Zero) }
        };
        var viejo = Movimiento("REEMBOLSO", 20, Luis, 3);
        viejo.IdMovimiento = 99;
        var ultimo = Movimiento("REEMBOLSO", 20, Dueno, 3);
        ultimo.IdMovimiento = 2;
        ultimo.FechaUtc = new(2026, 11, 20, 15, 0, 0, TimeSpan.Zero);
        ultimo.FechaLocal = new(2026, 11, 20, 10, 0, 0);

        SearchReservaPaginaMapper.Completar(filas, Array.Empty<Entity.DetalleReserva>(), new[] { viejo, ultimo }, Canchas);

        Assert.Equal("Dueño", filas[0].ResponsableUltimoReembolso);
        Assert.Equal(ultimo.FechaLocal, filas[0].FechaUltimoReembolso);
        Assert.Equal("Histórico sin responsable verificable", filas[1].ResponsableUltimoReembolso);
        Assert.Null(filas[1].FechaUltimoReembolso);
    }

    private static FiltroMovimientosReporteDto Filtro() => new() { FechaDesde = new(2026, 11, 13), FechaHasta = new(2026, 11, 13) };
    private static MovimientoReporteData Movimiento(string tipo, decimal importe, Guid? usuario, int cancha) => new()
    {
        IdMovimiento = 1, IdReserva = cancha == 3 ? 10 : 11, IdCancha = cancha, CodigoReserva = cancha == 3 ? "RES-10" : "RES-11", Tipo = tipo, Importe = importe,
        IdUsuarioResponsable = usuario, RolResponsable = usuario == Dueno ? Constants.Role.Proveedor : Constants.Role.Operador,
        NombreResponsable = usuario == Dueno ? "Dueño" : "Luis", NombreAuditoria = "No usar para identificar históricos",
        IdMetodoPago = 2, Metodo = "Efectivo", FechaLocal = new(2026, 11, 13, 10, 0, 0),
        FechaUtc = new(2026, 11, 13, 15, 0, 0, TimeSpan.Zero), FechaReservaLocal = new(2026, 11, 13)
    };
}
