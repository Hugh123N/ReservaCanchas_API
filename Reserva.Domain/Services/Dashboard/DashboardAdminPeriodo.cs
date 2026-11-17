using Reserva.Dto.Dbo.Dashboard;

namespace Reserva.Domain.Services.Dashboard;

public sealed class DashboardAdminPeriodo
{
    // Perú no cambia de horario; los límites UTC evitan depender del huso del servidor SQL.
    private static readonly TimeSpan OffsetPeru = TimeSpan.FromHours(-5);
    public DateTimeOffset Ahora { get; }
    public DateTimeOffset InicioMes { get; }
    public DateTimeOffset InicioMesAnterior => InicioMes.AddMonths(-1);
    public DateTimeOffset FinComparableAnterior { get; }
    public DateTimeOffset[] Inicios { get; }
    public DateTimeOffset HastaProximosSieteDias { get; }

    public DashboardAdminPeriodo(DateTimeOffset ahora, int meses)
    {
        if (meses is not (6 or 12)) throw new ArgumentOutOfRangeException(nameof(meses));
        Ahora = ahora.ToUniversalTime();
        var local = ahora.ToOffset(OffsetPeru);
        InicioMes = new DateTimeOffset(local.Year, local.Month, 1, 0, 0, 0, OffsetPeru).ToUniversalTime();
        FinComparableAnterior = local.AddMonths(-1).ToUniversalTime();
        HastaProximosSieteDias = Ahora.AddDays(7);
        Inicios = Enumerable.Range(0, meses).Select(i => InicioMes.AddMonths(i - meses + 1)).ToArray();
    }

    public List<DashboardAdminMesDto> CompletarMeses(IEnumerable<DashboardAdminMesValor> cobros, IEnumerable<DashboardAdminMesValor> registros)
    {
        var importes = cobros.ToDictionary(m => m.Indice, m => m.Valor);
        var cantidades = registros.ToDictionary(m => m.Indice, m => (int)m.Valor);
        return Inicios.Select((inicio, i) => new DashboardAdminMesDto
        {
            Anio = inicio.ToOffset(OffsetPeru).Year, Mes = inicio.ToOffset(OffsetPeru).Month,
            Cobrado = importes.GetValueOrDefault(i), NuevosProveedores = cantidades.GetValueOrDefault(i),
            EnCurso = i == Inicios.Length - 1
        }).ToList();
    }

    public static decimal? Variacion(decimal actual, decimal anterior) => anterior > 0
        ? Math.Round((actual - anterior) / anterior * 100, 1) : null;
}
