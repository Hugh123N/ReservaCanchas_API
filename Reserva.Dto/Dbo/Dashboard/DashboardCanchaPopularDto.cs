namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardCanchaPopularDto
    {
        /// <summary>Posición en el ranking (1, 2, 3...)</summary>
        public int Posicion { get; set; }

        /// <summary>Nombre de la cancha</summary>
        public string Nombre { get; set; } = null!;

        /// <summary>Total de reservas del mes actual</summary>
        public int TotalReservas { get; set; }

        /// <summary>Porcentaje respecto a la cancha con más reservas (0-100)</summary>
        public int Porcentaje { get; set; }

        /// <summary>Ingresos cobrados generados por esta cancha en el mes actual</summary>
        public decimal IngresosGenerados { get; set; }
    }
}
