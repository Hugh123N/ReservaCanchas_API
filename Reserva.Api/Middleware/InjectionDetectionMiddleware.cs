using Reserva.Common.Helpers;
using Reserva.Api.Security;
using System.Text;

namespace Reserva.Api.Middleware
{
    public class InjectionDetectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiProtectionOptions _options;

        private static readonly HashSet<string> SkipMethods = new(StringComparer.OrdinalIgnoreCase) { "GET", "HEAD", "OPTIONS" };

        public InjectionDetectionMiddleware(RequestDelegate next, ApiProtectionOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.InjectionDetectionEnabled)
            {
                await _next(context);
                return;
            }

            if (SkipMethods.Contains(context.Request.Method))
            {
                await _next(context);
                return;
            }

            bool tieneAmenaza;
            try { tieneAmenaza = await HasInjectionThreat(context.Request); }
            catch (InvalidDataException)
            {
                await RechazarAsync(context, "El formulario enviado no es válido o supera los límites permitidos.");
                return;
            }
            catch (BadHttpRequestException ex)
            {
                await RechazarAsync(context, "No se pudo leer el formulario enviado.", ex.StatusCode);
                return;
            }

            if (tieneAmenaza)
            {
                await RechazarAsync(context, "Solicitud rechazada: se detectó un patrón no permitido.");
                return;
            }

            await _next(context);
        }

        private static Task RechazarAsync(HttpContext context, string mensaje, int status = StatusCodes.Status400BadRequest)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(new
            {
                data = (object?)null,
                messages = new[] { new { message = mensaje, messageType = "Error" } },
                isValid = false
            });
        }

        private async Task<bool> HasInjectionThreat(HttpRequest request)
        {
            if (request.Query.Count > 0)
            {
                foreach (var (key, value) in request.Query)
                {
                    if (InjectionDetector.HasThreats(key) || InjectionDetector.HasThreats(value))
                        return true;
                }
            }

            if (request.HasFormContentType)
            {
                // ReadFormAsync conserva el formulario para el model binding de MVC. Los separadores,
                // las cabeceras multipart y los archivos binarios no son valores SQL/HTML del usuario.
                var form = await request.ReadFormAsync(request.HttpContext.RequestAborted);
                foreach (var (key, values) in form)
                {
                    if (InjectionDetector.HasThreats(key)) return true;
                    foreach (var value in values)
                        if (InjectionDetector.HasThreats(value)) return true;
                }
                return false;
            }

            if (request.ContentLength > 0 && request.ContentLength <= _options.MaxPayloadLength)
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                if (body.Length <= _options.MaxPayloadLength && InjectionDetector.HasThreats(body))
                    return true;
            }

            return false;
        }
    }
}
