using System.Text.RegularExpressions;

namespace Reserva.Common.Helpers
{
    public static class InjectionDetector
    {
        private static readonly Regex[] SqlPatterns =
        {
            new Regex(@"\b(EXEC\s*|EXECUTE\s*|XP_CMDSHELL)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\bUNION\s+(ALL\s+)?SELECT\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\bSELECT\b.*\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(DROP|ALTER|CREATE|TRUNCATE)\s+(TABLE|DATABASE|INDEX|VIEW|PROCEDURE|FUNCTION)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(INSERT|UPDATE|DELETE)\s+(INTO\s+)?\w+\s", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(WAITFOR|DELAY)\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(SHUTDOWN|KILL|RECONFIGURE|RESTORE|BACKUP)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(OR|AND)\s+\d+\s*=\s*\d+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(OR|AND)\s+['""]\w+['""]\s*=\s*['""]\w+['""]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b(CHAR|NCHAR|NVARCHAR|UNICODE|CONVERT|CAST)\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\b0x[0-9A-Fa-f]{4,}\b", RegexOptions.Compiled),
            new Regex(@"(/\*|\*/|--|;)", RegexOptions.Compiled),
            new Regex(@"\bDBMS_|UTL_|INFORMATION_SCHEMA\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\bSYSTEM_USER|CURRENT_USER|SESSION_USER|USER_NAME\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        };

        private static readonly Regex[] XssPatterns =
        {
            new Regex(@"<script[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<iframe[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<embed[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<object[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<svg[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<style[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<link[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<form[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"javascript\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"on(load|error|click|mouseover|submit|focus|blur|change|input|keydown|keyup)\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"expression\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"data\s*:\s*text/html", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<base[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<meta[^>]*http-equiv[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        };

        private const int MaxPayloadLength = 100000;

        public static bool HasThreats(string input)
        {
            return Analyze(input) != null;
        }

        public static string Analyze(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            if (input.Length > MaxPayloadLength)
                return "Payload exceeds maximum length";

            foreach (var pattern in SqlPatterns)
            {
                if (pattern.IsMatch(input))
                    return "Access denied";
            }

            foreach (var pattern in XssPatterns)
            {
                if (pattern.IsMatch(input))
                    return "Access denied";
            }

            return null;
        }
    }
}
