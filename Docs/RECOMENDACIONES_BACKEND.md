# Recomendaciones y Mejoras - Sistema de Reserva de Canchas

## ✅ Lo que se ha implementado exitosamente

### 1. **Sistema de Pre-Reserva Completo**
- ✅ Reservas en estado PENDIENTE con expiración automática
- ✅ Generación de códigos únicos (RES-2025-0001)
- ✅ Notificaciones por Email y WhatsApp
- ✅ Background Service para expiración automática
- ✅ Adelantos parciales configurables por cancha

### 2. **API REST bien estructurada**
- ✅ CQRS Pattern con MediatR
- ✅ Clean Architecture (separación de capas)
- ✅ FluentValidation para validaciones
- ✅ Generic Repository Pattern

### 3. **Notificaciones Duales**
- ✅ Email con templates HTML profesionales
- ✅ WhatsApp Cloud API integrado
- ✅ Manejo de errores que no afecta el flujo principal

---

## 🚀 Recomendaciones de Seguridad y Producción

### 1. **Autenticación y Autorización**

#### ⚠️ CRÍTICO: Proteger endpoints con JWT
Actualmente los endpoints no tienen `[Authorize]`. Debes agregar:

```csharp
// En ReservaController.cs
[Authorize] // Protege todos los endpoints del controller
public class ReservaController : IReservaApplication
{
    // Endpoints que solo el OPERADOR puede usar
    [Authorize(Roles = "Operador,Proveedor")]
    [HttpPost("confirmar-reserva-operador")]
    public async Task<ResponseDto<GetReservaDto>> ConfirmarReservaOperador(...)

    [Authorize(Roles = "Operador,Proveedor")]
    [HttpPost("liberar-reserva-operador")]
    public async Task<ResponseDto<GetReservaDto>> LiberarReservaOperador(...)

    [Authorize(Roles = "Operador,Proveedor")]
    [HttpGet("pendientes-operador/{idProveedor}")]
    public async Task<ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>> ObtenerReservasPendientesOperador(...)

    // Endpoint que solo el CLIENTE puede usar (sus propias reservas)
    [Authorize(Roles = "Cliente")]
    [HttpGet("mis-reservas/{idUsuario}")]
    public async Task<ResponseDto<IEnumerable<ReservaClienteDto>>> ObtenerReservasCliente(...)
}
```

#### ⚠️ Validar que el usuario solo acceda a SUS propios datos
```csharp
// En ReservasClienteQueryHandler.cs - Agregar validación
public async Task<ResponseDto<IEnumerable<ReservaClienteDto>>> Handle(...)
{
    // Obtener el ID del usuario autenticado desde el token JWT
    var usuarioAutenticado = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

    // Validar que el usuario solo solicite SUS propias reservas
    if (usuarioAutenticado != request.IdUsuario.ToString())
    {
        response.AddErrorResult("No tienes permiso para ver estas reservas.");
        return response;
    }

    // ... resto del código
}
```

### 2. **Validaciones Adicionales**

#### 📋 Validar que el operador pertenece al proveedor de la cancha
```csharp
// En ConfirmarReservaOperadorCommandHandler.cs - Agregar validación
var operadorAutorizado = await _OperadorRepository.ExistsAsync(
    o => o.IdUsuario == usuarioAutenticadoGuid
         && o.OperadorCancha.Any(oc => oc.IdCancha == reserva.IdCancha));

if (!operadorAutorizado)
{
    response.AddErrorResult("No tienes permisos para gestionar esta reserva.");
    return response;
}
```

#### 📋 Validar disponibilidad antes de confirmar
El operador podría confirmar una reserva cuyo horario ya fue tomado por otra reserva confirmada mientras estaba pendiente.

```csharp
// Validar que el horario sigue disponible antes de confirmar
var reservasConfirmadas = await _ReservaRepository.FindByAsNoTrackingAsync(
    r => r.IdCancha == reserva.IdCancha
         && r.Fecha.Date == reserva.Fecha.Date
         && r.IdReserva != reserva.IdReserva
         && r.Activo
         && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado,
    r => r.ReservaDetalle
);

// Verificar conflictos...
```

### 3. **Configuración de Producción**

#### 🔐 Usar Secrets Manager (NO appsettings.json)
Para producción, las credenciales sensibles deben estar en:
- **Azure Key Vault** (si usas Azure)
- **AWS Secrets Manager** (si usas AWS)
- **Variables de entorno** del servidor

```csharp
// En Program.cs
if (app.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

#### 📧 Configurar límites de envío (Rate Limiting)
WhatsApp Cloud API tiene límites:
- **1000 mensajes/día** en plan gratuito
- **Rate limit**: ~80 mensajes/segundo

```csharp
// En WhatsAppService.cs - Agregar control de rate limiting
private readonly SemaphoreSlim _rateLimiter = new(10, 10); // Max 10 concurrentes

public async Task<bool> SendTextMessageAsync(...)
{
    await _rateLimiter.WaitAsync();
    try
    {
        // ... enviar mensaje
    }
    finally
    {
        _rateLimiter.Release();
    }
}
```

### 4. **Monitoreo y Logs**

#### 📊 Agregar Application Insights o Serilog
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Elasticsearch
```

```csharp
// En Program.cs
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/reservas-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUri))
        {
            AutoRegisterTemplate = true,
            IndexFormat = "reservas-logs-{0:yyyy.MM}"
        }));
```

#### 📈 Métricas importantes a trackear
- Tasa de conversión: Pendiente → Confirmado
- Tasa de expiración de pre-reservas
- Tiempo promedio de confirmación
- Fallos en envío de notificaciones

### 5. **Base de Datos**

#### 🗄️ Índices adicionales para performance
```sql
-- Índice para búsquedas frecuentes de reservas por usuario
CREATE NONCLUSTERED INDEX IX_Reserva_IdUsuario_Fecha
ON [dbo].[Reserva] ([idUsuario], [fecha] DESC)
WHERE [activo] = 1;

-- Índice para búsquedas de operador
CREATE NONCLUSTERED INDEX IX_OperadorCancha_IdCancha
ON [dbo].[OperadorCancha] ([idCancha]);

-- Índice para background service (reservas a expirar)
CREATE NONCLUSTERED INDEX IX_Reserva_ProximasExpirar
ON [dbo].[Reserva] ([fechaExpiracionPreReserva], [idEstadoReserva])
WHERE [fechaExpiracionPreReserva] IS NOT NULL AND [activo] = 1;
```

#### 🔄 Implementar Soft Delete en Pago
Actualmente Pago tiene campo `Activo` pero no se usa consistentemente:
```csharp
// En vez de eliminar físicamente, marcar como inactivo
pago.Activo = false;
pago.UserNameUpdate = currentUser;
pago.UpdateDate = DateTimeOffset.UtcNow;
```

### 6. **Mejoras de Usuario**

#### 📱 Recordatorios automáticos
Crear un BackgroundService adicional para recordatorios:
- **24 horas antes**: "Tu reserva es mañana"
- **2 horas antes**: "Tu reserva es en 2 horas"

```csharp
public class ReservaReminderService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await EnviarRecordatorios24Horas();
            await EnviarRecordatorios2Horas();
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
```

#### 💳 Integrar pago anticipado con pasarela
Aunque el modelo actual es coordinación manual, podrías ofrecer:
- Pago anticipado OPCIONAL con Culqi
- Cliente paga online → Reserva confirmada automáticamente
- Si no paga online → Flujo actual (coordinación manual)

#### ⭐ Sistema de calificaciones
Después de cada reserva completada:
- Cliente califica la cancha (1-5 estrellas)
- Proveedor califica al cliente (opcional)
- Ayuda a otros usuarios a elegir

### 7. **Testing**

#### 🧪 Implementar Unit Tests
```csharp
// Ejemplo: Reserva.Domain.Tests/Commands/CreateReservaCommandHandlerTests.cs
public class CreateReservaCommandHandlerTests
{
    [Fact]
    public async Task Handle_CanchaNoExiste_DebeRetornarError()
    {
        // Arrange
        var handler = new CreateReservaCommandHandler(mockReservaRepo, ...);
        var command = new CreateReservaCommand(new CreateReservaDto { IdCancha = 999 });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("no existe", result.Message);
    }

    [Fact]
    public async Task Handle_HorarioOcupado_DebeRetornarError()
    {
        // ... test para conflicto de horarios
    }
}
```

#### 🧪 Integration Tests para endpoints
```csharp
public class ReservaControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Post_CrearReserva_DebeRetornar200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var reservaDto = new CreateReservaDto { ... };

        // Act
        var response = await client.PostAsJsonAsync("/api/Reserva", reservaDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ResponseDto<ReservaConPagoDto>>();
        Assert.NotNull(result.Data);
    }
}
```

---

## 📝 Cambios Menores Recomendados

### 1. **Agregar paginación al endpoint de cliente**
```csharp
// Modificar ReservasClienteQuery para soportar paginación
public class ReservasClienteQuery : IRequest<ResponseDto<PagedResult<ReservaClienteDto>>>
{
    public Guid IdUsuario { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? EstadoFiltro { get; set; } // "pendiente", "confirmada", "todas"
}
```

### 2. **Agregar campo para fotos de comprobantes**
```sql
ALTER TABLE [dbo].[Pago]
ADD [urlComprobante] VARCHAR(500) NULL;
```

El operador podría subir foto del recibo/voucher de pago.

### 3. **Webhook para actualización de estado en tiempo real**
Implementar SignalR para notificaciones en tiempo real al frontend:
```csharp
// Cuando se confirma una reserva
await _hubContext.Clients.User(reserva.IdUsuario.ToString())
    .SendAsync("ReservaConfirmada", reservaDto);
```

### 4. **Exportar reportes**
Endpoint para que el proveedor exporte sus reservas:
```csharp
[HttpGet("exportar-excel")]
public async Task<IActionResult> ExportarReservasExcel(int idProveedor, DateTime fechaInicio, DateTime fechaFin)
{
    // Generar Excel con EPPlus o ClosedXML
}
```

---

## 🎯 Prioridades para Producción

### Alta Prioridad (Hacer ANTES de producción):
1. ✅ **Agregar `[Authorize]` a todos los endpoints**
2. ✅ **Validar que usuarios solo accedan a SUS datos**
3. ✅ **Mover credenciales a Secrets Manager**
4. ✅ **Implementar Rate Limiting global en API**
5. ✅ **Agregar índices de base de datos recomendados**

### Media Prioridad (Primera versión de producción):
6. ⏳ Implementar logging con Serilog/Application Insights
7. ⏳ Agregar Health Checks (`/health` endpoint)
8. ⏳ Configurar CORS correctamente (solo dominios permitidos)
9. ⏳ Implementar retry policies para servicios externos (WhatsApp, Email)

### Baja Prioridad (Mejoras futuras):
10. 📅 Sistema de recordatorios automáticos
11. 📅 Sistema de calificaciones
12. 📅 Pago anticipado opcional con Culqi
13. 📅 SignalR para notificaciones en tiempo real
14. 📅 Exportar reportes Excel/PDF

---

## 📚 Documentación API (Swagger)

Agregar descripciones más completas en Swagger:

```csharp
// En Program.cs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Reserva Canchas API",
        Version = "v1",
        Description = "API para gestión de reservas de canchas deportivas",
        Contact = new OpenApiContact
        {
            Name = "Soporte",
            Email = "soporte@reservacanchas.com"
        }
    });

    // Habilitar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```

---

## 🔧 Comandos útiles para deployment

```bash
# Publicar API
dotnet publish -c Release -o ./publish

# Ejecutar migraciones en producción
dotnet ef database update --connection "Server=..."

# Verificar health
curl https://api.tudominio.com/health
```

---

## ✨ Conclusión

El backend está **sólido y bien estructurado**. Las implementaciones actuales son:
- ✅ Clean Architecture aplicada correctamente
- ✅ CQRS bien separado
- ✅ Notificaciones duales funcionando
- ✅ Background services para automatización

**Lo más crítico antes de producción** es agregar las capas de seguridad (autorización, validaciones de permisos) y mover credenciales a un sistema seguro.

¡El sistema está listo para el frontend! 🚀
