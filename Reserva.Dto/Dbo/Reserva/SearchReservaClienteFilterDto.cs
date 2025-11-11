namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO para filtros de búsqueda de reservas del cliente
    /// </summary>
    public class SearchReservaClienteFilterDto
    {
        /// <summary>
        /// Filtrar por estado de reserva (código): "01", "02", "03", "04"
        /// </summary>
        public string? CodigoEstado { get; set; }

        /// <summary>
        /// Filtrar por rango de fechas - desde
        /// </summary>
        public DateTimeOffset? FechaDesde { get; set; }

        /// <summary>
        /// Filtrar por rango de fechas - hasta
        /// </summary>
        public DateTimeOffset? FechaHasta { get; set; }

        /// <summary>
        /// Filtrar por estado de pago: "Pendiente", "Parcial", "Pagado"
        /// </summary>
        public string? EstadoPago { get; set; }

        /// <summary>
        /// Buscar por código de reserva
        /// </summary>
        public string? CodigoReserva { get; set; }

        /// <summary>
        /// Buscar por nombre de cancha
        /// </summary>
        public string? NombreCancha { get; set; }
    }
}
