namespace Reserva.Dto.Dbo.Dashboard;

public class GetDashboardAdminDto
{
    public DateTimeOffset GeneradoEn { get; set; }
    public string ZonaHoraria { get; set; } = "America/Lima";
    public string Moneda { get; set; } = "PEN";
    public int Meses { get; set; }
    public DashboardAdminIndicadoresDto Indicadores { get; set; } = new();
    public List<DashboardAdminMesDto> EvolucionMensual { get; set; } = new();
    public List<DashboardAdminPlanDto> ProveedoresPorPlan { get; set; } = new();
    public List<DashboardAdminAtencionDto> Atencion { get; set; } = new();
}

public class DashboardAdminIndicadoresDto
{
    public decimal CobradoMes { get; set; }
    public decimal CobradoMesAnteriorComparable { get; set; }
    public decimal? VariacionCobrosPorcentaje { get; set; }
    public int ProveedoresRegistrados { get; set; }
    public int ProveedoresActivos { get; set; }
    public int ProveedoresInactivos { get; set; }
    public int ProveedoresSuspendidos { get; set; }
    public int NuevosProveedoresMes { get; set; }
    public int ProveedoresPlanPago { get; set; }
    public int ProveedoresPrueba { get; set; }
    public int CanchasAprobadas { get; set; }
    public int CanchasPendientes { get; set; }
    public int PlanesEnGracia { get; set; }
    public int PlanesSuspendidos { get; set; }
}

public class DashboardAdminMesDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal Cobrado { get; set; }
    public int NuevosProveedores { get; set; }
    public bool EnCurso { get; set; }
}

public class DashboardAdminPlanDto
{
    public string Nombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal Porcentaje { get; set; }
}

public class DashboardAdminAtencionDto
{
    public string Codigo { get; set; } = "";
    public string Titulo { get; set; } = "";
    public int Cantidad { get; set; }
    public List<DashboardAdminCasoDto> Casos { get; set; } = new();
}

public class DashboardAdminCasoDto
{
    public string Proveedor { get; set; } = "";
    public string? Plan { get; set; }
    public string? Cancha { get; set; }
    public DateTimeOffset? Fecha { get; set; }
}
