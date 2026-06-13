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

            if (await HasInjectionThreat(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    data = (object?)null,
                    messages = new[]
                    {
                        new { message = "Solicitud rechazada: se detectó un patrón no permitido.", messageType = "Error" }
                    },
                    isValid = false
                });
                return;
            }

            await _next(context);
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
