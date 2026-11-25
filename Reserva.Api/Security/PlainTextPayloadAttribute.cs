namespace Reserva.Api.Security;

// Solo para endpoints que almacenan texto libre mediante consultas parametrizadas y lo presentan codificado.
[AttributeUsage(AttributeTargets.Method)]
public sealed class PlainTextPayloadAttribute : Attribute { }
