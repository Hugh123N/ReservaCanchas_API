namespace Reserva.Repository.Utils
{
    public static class TimezoneUtils
    {
        /// <summary>
        /// Mapeo de IANA timezone IDs a Windows timezone IDs para compatibilidad cross-platform.
        /// </summary>
        private static readonly Dictionary<string, string> IanaToWindows = new()
        {
            { "America/Lima", "SA Pacific Standard Time" },
            { "America/Bogota", "SA Pacific Standard Time" },
            { "America/Mexico_City", "Central Standard Time (Mexico)" },
            { "America/Santiago", "Pacific SA Standard Time" },
            { "America/Argentina/Buenos_Aires", "Argentina Standard Time" },
            { "America/Sao_Paulo", "E. South America Standard Time" },
            { "America/New_York", "Eastern Standard Time" },
            { "America/Chicago", "Central Standard Time" },
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "America/Caracas", "Venezuela Standard Time" },
            { "America/La_Paz", "SA Western Standard Time" },
            { "America/Asuncion", "Paraguay Standard Time" },
            { "America/Montevideo", "Montevideo Standard Time" },
            { "America/Guayaquil", "SA Pacific Standard Time" },
            { "America/Panama", "SA Pacific Standard Time" },
            { "America/Costa_Rica", "Central America Standard Time" },
            { "Europe/Madrid", "Romance Standard Time" },
            { "Europe/London", "GMT Standard Time" },
            { "UTC", "UTC" },
        };

        /// <summary>
        /// Obtiene TimeZoneInfo a partir de un IANA timezone ID (ej: "America/Lima").
        /// Funciona en Windows y Linux.
        /// </summary>
        public static TimeZoneInfo ObtenerZonaHoraria(string zonaHorariaId)
        {
            if (string.IsNullOrWhiteSpace(zonaHorariaId))
                return GetDefaultTimezone();

            // Intentar directamente (funciona en Linux y .NET 6+ en Windows)
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(zonaHorariaId);
            }
            catch (TimeZoneNotFoundException)
            {
                // En Windows antiguo, intentar con el mapeo IANA → Windows
                if (IanaToWindows.TryGetValue(zonaHorariaId, out var windowsId))
                {
                    try
                    {
                        return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        // Fallback
                    }
                }
            }

            // Último recurso: timezone por defecto
            return GetDefaultTimezone();
        }

        /// <summary>
        /// Obtiene la zona horaria por defecto del sistema (America/Lima o equivalente).
        /// </summary>
        public static TimeZoneInfo GetDefaultTimezone()
        {
            return GetEasternOrPacific();
        }

        /// <summary>
        /// Detecta automáticamente una zona horaria SA Pacific (Lima/Bogota) del sistema.
        /// </summary>
        public static TimeZoneInfo GetEasternOrPacific()
        {
            try
            {
                #region Linux
                if (TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "America/Lima"))
                    return TimeZoneInfo.FindSystemTimeZoneById("America/Lima");

                if (TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "America/Bogota"))
                    return TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

                if (TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "America/New_York"))
                    return TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

                return TimeZoneInfo.Utc;
                #endregion
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    #region Windows
                    if (TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "SA Pacific Standard Time"))
                        return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                    if (TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "Eastern Standard Time"))
                        return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                    if (TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "US Eastern Standard Time"))
                        return TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");

                    return TimeZoneInfo.Utc;
                    #endregion
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.Utc;
                }
            }
        }
    }
}
