namespace Reserva.Repository.Utils
{
    /// <summary>
    /// Helper global para operaciones con fechas y horas.
    /// Proporciona métodos reutilizables en todo el proyecto para manejar fechas de manera consistente.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Normaliza una fecha a UTC con hora 00:00:00 para comparaciones consistentes
        /// sin importar la zona horaria del servidor o cliente.
        /// Esto evita problemas donde una misma fecha puede cambiar de día al convertir entre timezones.
        /// </summary>
        /// <param name="fecha">Fecha a normalizar</param>
        /// <returns>Fecha normalizada a UTC con hora 00:00:00</returns>
        /// <example>
        /// var fecha = new DateTimeOffset(2026, 3, 14, 15, 30, 0, TimeSpan.FromHours(-5));
        /// var normalizada = DateTimeHelper.NormalizarFechaUtc(fecha);
        /// // Resultado: 2026-03-14T00:00:00+00:00 (UTC)
        /// </example>
        public static DateTimeOffset NormalizarFechaUtc(DateTimeOffset fecha)
        {
            // Convertir a UTC y tomar solo año, mes, día (ignorar hora y offset original)
            var fechaUtc = fecha.UtcDateTime;
            return new DateTimeOffset(fechaUtc.Year, fechaUtc.Month, fechaUtc.Day, 0, 0, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// Normaliza un DateTime a UTC con hora 00:00:00 para comparaciones consistentes.
        /// </summary>
        /// <param name="fecha">Fecha a normalizar</param>
        /// <returns>Fecha normalizada a UTC con hora 00:00:00</returns>
        public static DateTimeOffset NormalizarFechaUtc(DateTime fecha)
        {
            // Convertir a UTC (asumiendo que es local si no tiene Kind especificado)
            DateTime fechaUtc = fecha.Kind == DateTimeKind.Utc
                ? fecha
                : fecha.ToUniversalTime();

            return new DateTimeOffset(fechaUtc.Year, fechaUtc.Month, fechaUtc.Day, 0, 0, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// Compara dos fechas ignorando la hora y zona horaria.
        /// Útil para verificar si dos fechas son el mismo día.
        /// </summary>
        /// <param name="fecha1">Primera fecha</param>
        /// <param name="fecha2">Segunda fecha</param>
        /// <returns>true si son el mismo día, false si no</returns>
        public static bool SonMismoDia(DateTimeOffset fecha1, DateTimeOffset fecha2)
        {
            var f1 = NormalizarFechaUtc(fecha1);
            var f2 = NormalizarFechaUtc(fecha2);
            return f1.Date == f2.Date;
        }
    }
}
