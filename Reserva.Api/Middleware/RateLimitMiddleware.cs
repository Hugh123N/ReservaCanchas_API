using System.Collections.Concurrent;
using System.Net;
using Reserva.Api.Security;

namespace Reserva.Api.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiProtectionOptions _options;
        private readonly ILogger<RateLimitMiddleware> _logger;

        private static readonly ConcurrentDictionary<string, SlidingWindow> _windows = new();
        private static readonly Timer _cleanupTimer;

        private static readonly HashSet<string> StaticExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".js", ".css", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg",
            ".woff", ".woff2", ".ttf", ".eot", ".map", ".json",
            ".html", ".htm"
        };

        static RateLimitMiddleware()
        {
            _cleanupTimer = new Timer(_ => Cleanup(), null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public RateLimitMiddleware(RequestDelegate next, ApiProtectionOptions options, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.RateLimitEnabled)
            {
                await _next(context);
                return;
            }

            if (ShouldSkip(context))
            {
                await _next(context);
                return;
            }

            var ip = GetClientIp(context);
            if (string.IsNullOrEmpty(ip))
            {
                await _next(context);
                return;
            }

            var window = _windows.GetOrAdd(ip, _ => new SlidingWindow());
            var now = DateTime.UtcNow;

            lock (window)
            {
                window.RemoveExpired(now, _options.RateLimitWindowSeconds);

                if (window.IsBlocked(now))
                {
                    var retryAfter = (int)(window.BlockedUntil!.Value - now).TotalSeconds;
                    if (retryAfter < 0)
                    {
                        window.ClearBlock();
                    }
                    else
                    {
                        SetRateLimitHeaders(context, 0, retryAfter);
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        context.Response.ContentType = "application/json";
                        context.Response.Headers.RetryAfter = retryAfter.ToString();
                        _logger.LogWarning("Rate limit exceeded for IP: {Ip}", ip);
                        return;
                    }
                }

                if (window.Timestamps.Count >= _options.RateLimitMaxRequests)
                {
                    window.BlockedUntil = now.AddSeconds(_options.RateLimitBlockSeconds);
                    window.Timestamps.Clear();
                    SetRateLimitHeaders(context, 0, _options.RateLimitBlockSeconds);
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.RetryAfter = _options.RateLimitBlockSeconds.ToString();
                    _logger.LogWarning("IP blocked for rate limit: {Ip}", ip);
                    return;
                }

                window.Timestamps.Add(now);
                var remaining = _options.RateLimitMaxRequests - window.Timestamps.Count;
                SetRateLimitHeaders(context, remaining);
            }

            await _next(context);
        }

        private void SetRateLimitHeaders(HttpContext context, int remaining, int? retryAfter = null)
        {
            context.Response.Headers["X-RateLimit-Limit"] = _options.RateLimitMaxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            context.Response.Headers["X-RateLimit-Window"] = _options.RateLimitWindowSeconds + "s";

            if (retryAfter.HasValue)
            {
                context.Response.Headers["X-RateLimit-Reset"] = retryAfter + "s";
            }
        }

        private static string? GetClientIp(HttpContext context)
        {
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
            {
                var ip = forwarded.Split(',')[0].Trim();
                if (IPAddress.TryParse(ip, out _))
                    return ip;
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }

        private static bool ShouldSkip(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var lastDot = path.LastIndexOf('.');
            if (lastDot >= 0)
            {
                var ext = path[lastDot..];
                if (StaticExtensions.Contains(ext))
                    return true;
            }

            return false;
        }

        private static void Cleanup()
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in _windows)
            {
                lock (kvp.Value)
                {
                    if (kvp.Value.Timestamps.Count == 0 && !kvp.Value.IsBlocked(now))
                        _windows.TryRemove(kvp.Key, out _);
                }
            }
        }

        private class SlidingWindow
        {
            public List<DateTime> Timestamps { get; } = new();
            public DateTime? BlockedUntil { get; set; }

            public bool IsBlocked(DateTime now) => BlockedUntil.HasValue && BlockedUntil.Value > now;

            public void ClearBlock() => BlockedUntil = null;

            public void RemoveExpired(DateTime now, int windowSeconds)
            {
                var cutoff = now.AddSeconds(-windowSeconds);
                Timestamps.RemoveAll(t => t < cutoff);
            }
        }
    }
}
