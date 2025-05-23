namespace Reserva.Repository.Utils
{
    public static class TimezoneUtils
    {
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
