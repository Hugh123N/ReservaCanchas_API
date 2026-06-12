using System;
using System.Text.RegularExpressions;

namespace Reserva.Common.Helpers
{
    public static class Sanitizer
    {
        // Caracteres de control prohibidos (excepto \t \n \r que son \x09 \x0A \x0D)
        private static readonly Regex ControlChars = new Regex(
            @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]",
            RegexOptions.Compiled
        );

        // Allowlist: letras, dígitos, acentos latinos, símbolos comunes + separadores de protocolo
        private static readonly Regex Allowlist = new Regex(
            @"[^a-zA-Z0-9" +
            @"áéíóúÁÉÍÓÚ" +
            @"àèìòùÀÈÌÒÙ" +
            @"âêîôûÂÊÎÔÛ" +
            @"äëïöüÄËÏÖÜ" +
            @"ñÑçÇãõÃÕ" +
            @"@.\-_,:/()+=%\s" +
            @"¿?¡!$&*'" + "\"" +
            @"\[\]{}<>#" +
            @"]",
            RegexOptions.Compiled
        );

        // Detecta etiquetas HTML/XML
        private static readonly Regex HtmlTags = new Regex(
            @"<[^>]*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Detecta intentos de SQL Injection básicos
        private static readonly Regex SqlInjection = new Regex(
            @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|CAST|CONVERT|DECLARE|TRUNCATE|MERGE|REPLACE)\b)|(--)|(;)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public static string Clean(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. Eliminar caracteres de control
            input = ControlChars.Replace(input, "");

            // 2. Eliminar caracteres fuera de la allowlist
            input = Allowlist.Replace(input, "");

            return input.Trim();
        }

        public static string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return HtmlTags.Replace(input, "");
        }

        public static string CleanAndStripHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = StripHtml(input);
            return Clean(input);
        }

        public static bool HasSqlInjectionRisk(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return SqlInjection.IsMatch(input);
        }

        public static string CleanFull(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = CleanAndStripHtml(input);

            if (HasSqlInjectionRisk(input))
                throw new InvalidOperationException("Input contiene patrones no permitidos.");

            return input;
        }
    }
}