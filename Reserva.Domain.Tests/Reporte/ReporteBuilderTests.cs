using Reserva.Common;
using Reserva.Domain.Queries.Reporte;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Reporte;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class ReporteBuilderTests
{
    private static readonly DateOnly Dia = new(2026, 11, 13);
    private static readonly List<CanchaReporteDto> Canchas = new() { new(3, "Cancha 3", "America/Lima", true) };

    [Fact]
    public void Economico_SeparaImporteVigenteReembolsosNetoYSaldo()
    {
        var reservas = new List<ReporteReservaData>
        {
            Reserva(1, Constants.ESTADO_RESERVA.Confirmado, 100, 50),
            Reserva(2, Constants.ESTADO_RESERVA.Cancelado, 100, 100, 70),
            Reserva(3, Constants.ESTADO_RESERVA.Pendiente, 200, 0)
        };
        var reporte = ReporteBuilder.Construir(TipoReporte.Economico, Filtro(), reservas, new(), Canchas, new());
        decimal Valor(string etiqueta) => reporte.Indicadores.Single(i => i.Etiqueta == etiqueta).Valor;
        Assert.Equal(300, Valor("Importe vigente"));
        Assert.Equal(150, Valor("Cobrado acumulado"));
        Assert.Equal(70, Valor("Devuelto"));
        Assert.Equal(80, Valor("Cobrado neto"));
        Assert.Equal(250, Valor("Saldo vigente"));
        Assert.Equal(0m, reporte.Filas.Single(f => (string)f["codigo"]! == "RES-2")["saldo"]);
    }

    [Fact]
    public void Economico_UsaElAcumuladoCorregidoAunqueNoExistaHistorialDeCobros()
    {
        var r = Reserva(1, Constants.ESTADO_RESERVA.Confirmado, 100, 40);
        var reporte = ReporteBuilder.Construir(TipoReporte.Economico, Filtro(), new() { r }, new(), Canchas, new());
        Assert.Equal(40m, reporte.Filas[0]["cobrado"]);
        Assert.Equal(60m, reporte.Filas[0]["saldo"]);
        Assert.Equal(Dia, reporte.Filas[0]["fecha"]);
    }

    [Fact]
    public void Anuladas_NoSumanSaldoNiImporteVigente()
    {
        var r = Reserva(1, Constants.ESTADO_RESERVA.Pendiente, 100, 0);
        r.Activo = false;
        var reporte = ReporteBuilder.Construir(TipoReporte.Economico, Filtro(), new() { r }, new(), Canchas, new());
        Assert.Equal(0, reporte.Indicadores.Single(i => i.Etiqueta == "Importe vigente").Valor);
        Assert.Equal(0m, reporte.Filas[0]["saldo"]);
    }

    [Fact]
    public void Demanda_NoCuentaSlotsComoHorasNiDuplicaReservasEnElResumen()
    {
        var reservas = new List<ReporteReservaData> { Reserva(1, Constants.ESTADO_RESERVA.Confirmado, 100, 50),
            Reserva(2, Constants.ESTADO_RESERVA.Pendiente, 100, 0), Reserva(3, Constants.ESTADO_RESERVA.Cancelado, 100, 0) };
        var horarios = new List<ReporteHorarioData>
        {
            new(1, 3, 10, new(10, 0), new(10, 30)), new(1, 3, 11, new(10, 30), new(11, 0)),
            new(2, 3, 10, new(10, 0), new(10, 30)), new(3, 3, 10, new(10, 0), new(10, 30))
        };
        var franjas = new List<FranjaReporteData> { new(3, 5, new(14, 0), new(15, 0)) };
        var reporte = ReporteBuilder.Construir(TipoReporte.Demanda, Filtro(), reservas, horarios, Canchas, franjas);
        Assert.Equal(1, reporte.Indicadores.Single(i => i.Etiqueta == "Reservas confirmadas").Valor);
        Assert.Equal(1, reporte.Indicadores.Single(i => i.Etiqueta == "Horas confirmadas").Valor);
        var primera = reporte.Filas.Single(f => (string)f["inicio"]! == "10:00");
        Assert.Equal(0.5m, primera["horasConfirmadas"]);
        Assert.Equal(0.5m, primera["horasPendientes"]);
        Assert.Equal(1, primera["canceladas"]);
        Assert.Equal(0m, reporte.Filas.Single(f => (string)f["inicio"]! == "14:00")["horasConfirmadas"]);
    }

    [Theory]
    [InlineData(23, 0, 1, 0, 2)]
    [InlineData(10, 0, 10, 30, 0.5)]
    public void Duracion_RespetaMedianocheYMediaHora(int hi, int mi, int hf, int mf, double horas)
        => Assert.Equal((decimal)horas, new ReporteHorarioData(1, 3, 10, new(hi, mi), new(hf, mf)).Horas);

    [Fact]
    public void Agenda_AgrupaLosHorariosConsecutivosYMuestraSaldosVigentes()
    {
        var reporte = ReporteBuilder.Construir(TipoReporte.AgendaOperador, Filtro(),
            new() { Reserva(1, Constants.ESTADO_RESERVA.Confirmado, 100, 40) },
            new() { new(1, 3, 10, new(10, 0), new(10, 30)), new(1, 3, 11, new(10, 30), new(11, 0)) }, Canchas, new());
        Assert.Equal("10:00 - 11:00", reporte.Filas[0]["horario"]);
        Assert.Equal(60, reporte.Indicadores.Single(i => i.Etiqueta == "Por cobrar").Valor);
        Assert.Contains(reporte.Columnas, c => c.Campo == "telefono");
    }

    [Fact]
    public async Task Validaciones_NoAceptanRangosInvalidosNiAgendaDeVariosDias()
    {
        var validator = new GetReporteQueryValidator();
        var filtro = Filtro();
        filtro.FechaHasta = Dia.AddDays(-1);
        Assert.False((await validator.ValidateAsync(new GetReporteQuery(TipoReporte.Economico, filtro))).IsValid);
        filtro.FechaHasta = Dia.AddDays(366);
        Assert.False((await validator.ValidateAsync(new GetReporteQuery(TipoReporte.Economico, filtro))).IsValid);
        filtro.FechaHasta = Dia.AddDays(1);
        Assert.False((await validator.ValidateAsync(new GetReporteQuery(TipoReporte.AgendaOperador, filtro))).IsValid);
        filtro.FechaHasta = Dia;
        filtro.EstadoReserva = "VIGENTES";
        Assert.False((await validator.ValidateAsync(new GetReporteQuery(TipoReporte.Demanda, filtro))).IsValid);
    }

    private static FiltroReporteDto Filtro() => new() { FechaDesde = Dia, FechaHasta = Dia };
    private static ReporteReservaData Reserva(int id, string estado, decimal total, decimal cobrado, decimal devuelto = 0) => new()
    {
        IdReserva = id, IdCancha = 3, Codigo = $"RES-{id}", FechaLocal = Dia, HoraLocal = new(10, 0),
        Activo = true, EstadoCodigo = estado, EstadoNombre = estado, Cliente = "Cliente",
        Pago = new ReportePagoData { Total = total, Cobrado = cobrado, Devuelto = devuelto }
    };
}
