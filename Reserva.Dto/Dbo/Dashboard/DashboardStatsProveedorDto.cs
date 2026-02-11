namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardStatsProveedorDto
    {
        /// <summary>Ingresos cobrados en el mes actual</summary>
        public decimal IngresosMesActual { get; set; }

        /// <summary>Variación porcentual de ingresos vs mes anterior</summary>
        public decimal VariacionIngresosMes { get; set; }

        /// <summary>Total de reservas del mes actual (todos los estados)</summary>
        public int ReservasMes { get; set; }

        /// <summary>Reservas pendientes de atención (cualquier fecha)</summary>
        public int ReservasPendientesAtencion { get; set; }

        /// <summary>Reservas para el día de hoy</summary>
        public int ReservasHoy { get; set; }

        /// <summary>Tasa de ocupación promedio del mes actual (%)</summary>
        public double TasaOcupacionPromedio { get; set; }
    }
}
