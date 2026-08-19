namespace Reserva.Repository.Utils
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Calcula la fecha de próximo cobro basada en la duración del plan.
        /// Mapea duración en días a meses: 30→1, 90→3, 365→12.
        /// </summary>
        public static DateTimeOffset GetNextBillingDate(
            DateTimeOffset currentDate,
            int billingDay,
            int durationDays)
        {
            int monthsToAdd = durationDays switch
            {
                30 => 1,    // Mensual
                90 => 3,    // Trimestral
                365 => 12,  // Anual
                _ => throw new ArgumentException(
                    $"Duración no soportada: {durationDays} días")
            };

            var nextMonth = currentDate.AddMonths(monthsToAdd);

            var lastDay = DateTime.DaysInMonth(
                nextMonth.Year,
                nextMonth.Month
            );

            var day = Math.Min(billingDay, lastDay);

            return new DateTimeOffset(
                nextMonth.Year,
                nextMonth.Month,
                day,
                currentDate.Hour,
                currentDate.Minute,
                currentDate.Second,
                currentDate.Offset
            );
        }
        /// <summary>
        /// Normaliza una fecha a la zona horaria local de la cancha, con hora 00:00:00.
        /// Esto garantiza que "18 de marzo 11pm en Lima (UTC-5)" se normalice como 18 de marzo,
        /// no como 19 de marzo (que sería si se convirtiera a UTC primero).
        /// </summary>
        public static DateTimeOffset NormalizarFechaLocal(DateTimeOffset fecha, TimeZoneInfo zonaHoraria)
        {
            var fechaLocal = TimeZoneInfo.ConvertTime(fecha, zonaHoraria);
            return new DateTimeOffset(fechaLocal.Year, fechaLocal.Month, fechaLocal.Day,
                0, 0, 0, fechaLocal.Offset);
        }

        /// <summary>
        /// Normaliza un DateTime a la zona horaria local, con hora 00:00:00.
        /// </summary>
        public static DateTimeOffset NormalizarFechaLocal(DateTime fecha, TimeZoneInfo zonaHoraria)
        {
            DateTime fechaUnspec = DateTime.SpecifyKind(fecha, DateTimeKind.Unspecified);
            var offset = zonaHoraria.GetUtcOffset(fechaUnspec);
            var fechaLocal = new DateTimeOffset(fechaUnspec, offset);
            return new DateTimeOffset(fechaLocal.Year, fechaLocal.Month, fechaLocal.Day,
                0, 0, 0, offset);
        }

        /// <summary>
        /// Obtiene la fecha y hora actual en la zona horaria especificada.
        /// </summary>
        public static DateTimeOffset ObtenerAhoraLocal(TimeZoneInfo zonaHoraria)
        {
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, zonaHoraria);
        }

        /// <summary>
        /// Convierte un DateTimeOffset a la zona horaria local especificada.
        /// </summary>
        public static DateTimeOffset ConvertirALocal(DateTimeOffset fecha, TimeZoneInfo zonaHoraria)
        {
            return TimeZoneInfo.ConvertTime(fecha, zonaHoraria);
        }

        /// <summary>
        /// Compara dos fechas ignorando la hora, usando la zona horaria local de la cancha.
        /// </summary>
        public static bool SonMismoDia(DateTimeOffset fecha1, DateTimeOffset fecha2, TimeZoneInfo zonaHoraria)
        {
            var f1 = NormalizarFechaLocal(fecha1, zonaHoraria);
            var f2 = NormalizarFechaLocal(fecha2, zonaHoraria);
            return f1.Date == f2.Date;
        }

        /// <summary>
        /// Obtiene la fecha local (solo la parte Date) de un DateTimeOffset en la zona horaria dada.
        /// </summary>
        public static DateTime ObtenerFechaLocal(DateTimeOffset fecha, TimeZoneInfo zonaHoraria)
        {
            var fechaLocal = TimeZoneInfo.ConvertTime(fecha, zonaHoraria);
            return fechaLocal.Date;
        }

        // =====================================================
        // Métodos legacy (sin timezone) - mantener compatibilidad
        // =====================================================

        /// <summary>
        /// [LEGACY] Normaliza una fecha a UTC con hora 00:00:00.
        /// Preferir NormalizarFechaLocal() con la zona horaria de la cancha.
        /// </summary>
        public static DateTimeOffset NormalizarFechaUtc(DateTimeOffset fecha)
        {
            var fechaUtc = fecha.UtcDateTime;
            return new DateTimeOffset(fechaUtc.Year, fechaUtc.Month, fechaUtc.Day, 0, 0, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// [LEGACY] Normaliza un DateTime a UTC con hora 00:00:00.
        /// Preferir NormalizarFechaLocal() con la zona horaria de la cancha.
        /// </summary>
        public static DateTimeOffset NormalizarFechaUtc(DateTime fecha)
        {
            DateTime fechaUtc = fecha.Kind == DateTimeKind.Utc
                ? fecha
                : fecha.ToUniversalTime();

            return new DateTimeOffset(fechaUtc.Year, fechaUtc.Month, fechaUtc.Day, 0, 0, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// [LEGACY] Compara dos fechas ignorando la hora y zona horaria.
        /// Preferir SonMismoDia() con la zona horaria de la cancha.
        /// </summary>
        public static bool SonMismoDia(DateTimeOffset fecha1, DateTimeOffset fecha2)
        {
            var f1 = NormalizarFechaUtc(fecha1);
            var f2 = NormalizarFechaUtc(fecha2);
            return f1.Date == f2.Date;
        }
    }
}
