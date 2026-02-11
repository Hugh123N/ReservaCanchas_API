namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardIngresoMensualDto
    {
        /// <summary>Nombre corto del mes (Ene, Feb, Mar...)</summary>
        public string Mes { get; set; } = null!;

        /// <summary>Monto cobrado en el mes</summary>
        public decimal Monto { get; set; }
    }
}
