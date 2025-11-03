# 📚 Documentación Completa - Backend Sistema de Reserva de Canchas

## 🎯 Resumen Ejecutivo

Este documento detalla las **5 FASES** implementadas en el backend para el sistema de pre-reserva y pago de canchas deportivas.

**Modelo de Negocio Actual:**
- Cliente crea **pre-reserva** (estado PENDIENTE) - Solo pago en EFECTIVO
- Sistema genera código único y notifica al operador
- Operador contacta al cliente y coordina pago en efectivo
- Operador confirma la reserva (estado CONFIRMADO) con adelanto opcional
- Si no se confirma a tiempo, la reserva EXPIRA automáticamente

---

## 📋 FASE 1: Refactorización de Base de Datos y Entidades

### Objetivo
Adaptar el modelo de datos para soportar pre-reservas con expiración automática y adelantos parciales.

### Cambios en la Base de Datos

#### Tabla: `Cancha`
```sql
ALTER TABLE [dbo].[Cancha]
ADD [duracionPreReserva] INT NULL,              -- Horas antes de expirar (ej: 24, 48)
    [porcentajeAdelanto] DECIMAL(5,2) NULL,     -- % mínimo de adelanto (ej: 50.00)
    [telefonoCancha] VARCHAR(20) NULL;          -- Teléfono directo de la cancha
```

**Valores por defecto:**
- `duracionPreReserva`: 48 horas
- `porcentajeAdelanto`: 50%

#### Tabla: `Reserva`
```sql
ALTER TABLE [dbo].[Reserva]
ADD [codigoReserva] VARCHAR(50) NULL,                         -- Código único (RES-2025-0001)
    [fechaExpiracionPreReserva] DATETIMEOFFSET NULL,          -- Fecha límite para confirmar
    [notificacionAdvertenciaEnviada] BIT NOT NULL DEFAULT 0;  -- Flag para evitar duplicados
```

**Índices creados:**
```sql
-- Índice único para código de reserva
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Reserva_CodigoReserva]
ON [dbo].[Reserva] ([codigoReserva])
WHERE [codigoReserva] IS NOT NULL;

-- Índice para búsquedas por fecha de expiración (usado por background service)
CREATE NONCLUSTERED INDEX [IX_Reserva_FechaExpiracion]
ON [dbo].[Reserva] ([fechaExpiracionPreReserva], [idEstadoReserva])
WHERE [fechaExpiracionPreReserva] IS NOT NULL;
```

#### Tabla: `EstadoReserva`
```sql
INSERT INTO [dbo].[EstadoReserva] ([codigo], [nombre], [activo])
VALUES ('04', 'Expirado', 1);
```

**Estados completos:**
- `01` - Pendiente (pre-reserva sin confirmar)
- `02` - Confirmado (reserva confirmada por operador)
- `03` - Cancelado (liberada manualmente por operador)
- `04` - Expirado (venció el tiempo límite sin confirmación)

#### Stored Procedure: `sp_GenerarCodigoReserva`
```sql
CREATE PROCEDURE [dbo].[sp_GenerarCodigoReserva]
AS
BEGIN
    -- Genera código único: RES-{AÑO}-{NÚMERO}
    -- Ejemplo: RES-2025-0001, RES-2025-0002, etc.
    -- Se resetea cada año
END
```

### Cambios en las Entidades (C#)

#### `Cancha.cs`
```csharp
public partial class Cancha
{
    // ... campos existentes

    /// <summary>
    /// Duración en horas de la pre-reserva antes de expirar
    /// </summary>
    public int? DuracionPreReserva { get; set; }

    /// <summary>
    /// Porcentaje mínimo de adelanto requerido (0-100)
    /// </summary>
    public decimal? PorcentajeAdelanto { get; set; }

    public string? TelefonoCancha { get; set; }
}
```

#### `Reserva.cs`
```csharp
public partial class Reserva
{
    // ... campos existentes

    /// <summary>
    /// Código único legible de la reserva
    /// </summary>
    public string CodigoReserva { get; set; } = null!;

    /// <summary>
    /// Fecha y hora límite antes de expirar
    /// </summary>
    public DateTimeOffset? FechaExpiracionPreReserva { get; set; }

    /// <summary>
    /// Flag para evitar notificaciones duplicadas de proximidad
    /// </summary>
    public bool NotificacionAdvertenciaEnviada { get; set; }
}
```

### Script de Migración
**Archivo:** `SQLReservaCanchasV3_MigracionPreReserva.sql`

Incluye:
- Creación de campos nuevos con validaciones
- Generación automática de códigos para reservas existentes
- Creación de stored procedure
- Índices para performance
- Verificación y estadísticas al final

---

## 📋 FASE 2: Refactorización del Flujo de Creación de Reservas

### Objetivo
Eliminar métodos de pago digitales (Yape, Plin, Transferencia) para clientes y establecer flujo de pre-reserva con coordinación manual.

### Cambios Importantes

#### ❌ ELIMINADO: Strategy Pattern de Pagos
**Antes:**
- PagoStrategyFactory
- YapePagoStrategy
- PlinPagoStrategy
- TransferenciaPagoStrategy
- Integración con pasarelas de pago

**Ahora:**
- Solo método de pago: **EFECTIVO**
- Coordinación manual entre operador y cliente
- Pago se registra cuando el operador confirma

#### `CreateReservaCommandHandler.cs` - Refactorizado

**Flujo nuevo:**

1. **Validar método de pago (SOLO EFECTIVO)**
```csharp
if (metodoPago.Codigo != Constants.METODO_PAGO.Efectivo)
{
    response.AddErrorResult($"Solo se acepta pago en efectivo. El método '{metodoPago.Nombre}' no está disponible.");
    return response;
}
```

2. **Validar disponibilidad de horario**
```csharp
var reservasDelDia = await _ReservaRepository.FindByAsNoTrackingAsync(
    r => r.IdCancha == request.CreateDto.IdCancha
         && r.Fecha.Date == request.CreateDto.Fecha.Date
         && r.Activo
         && r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Cancelado
         && r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Expirado
);

// Verificar conflictos de horario...
```

3. **Crear reserva en estado PENDIENTE**
```csharp
var estadoPendienteReserva = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
    x => x.Codigo == Constants.ESTADO_RESERVA.Pendiente);

nuevaReserva.IdEstadoReserva = estadoPendienteReserva.IdEstadoReserva;
```

4. **Calcular fecha de expiración**
```csharp
int duracionPreReservaHoras = cancha.DuracionPreReserva ?? 24;
nuevaReserva.FechaExpiracionPreReserva = DateTimeOffset.Now.AddHours(duracionPreReservaHoras);
```

5. **Generar código único**
```csharp
nuevaReserva.CodigoReserva = await GenerarCodigoReserva();
// Formato: RES-2025-0001
```

6. **Crear pago en estado PENDIENTE**
```csharp
var nuevoPago = new Entity.Pago
{
    IdReserva = nuevaReserva.IdReserva,
    Moneda = "PEN",
    Monto = montoTotal,
    MontoAdelanto = 0,
    MontoPendiente = montoTotal,
    IdMetodoPago = metodoPago.IdMetodoPago,
    IdEstadoPago = estadoPagoPendiente.IdEstadoPago
};
```

7. **Obtener operadores de la cancha**
```csharp
var operadores = await _OperadorRepository.FindByAsNoTrackingAsync(
    x => x.OperadorCancha.Any(c => c.IdCancha == cancha.IdCancha),
    x => x.IdUsuarioNavigation
);
```

8. **Enviar notificaciones**
```csharp
await _notificacionService.NotificarNuevaReservaPendienteAsync(
    nuevaReserva,
    cancha,
    cliente,
    operadores.ToList()
);
```

### DTOs Modificados

#### `ReservaConPagoDto` - Respuesta al cliente
```csharp
public class ReservaConPagoDto
{
    public GetReservaDto Reserva { get; set; }
    public GetPagoDto Pago { get; set; }

    // Información del operador para contacto
    public string TelefonoCancha { get; set; }
    public string NombreOperador { get; set; }

    // Información del pago
    public string MetodoPago { get; set; }
    public string MontoFormateado { get; set; }
    public string Moneda { get; set; }

    // Información de la pre-reserva
    public string CodigoReserva { get; set; }
    public int DuracionPreReservaHoras { get; set; }
    public DateTimeOffset? FechaExpiracionPreReserva { get; set; }

    public string InformacionAdicional { get; set; }
    // Ejemplo: "Tu reserva ha sido creada con el código RES-2025-0001.
    //           El encargado se comunicará contigo para coordinar el pago.
    //           La pre-reserva expira el 15/01/2025 14:30."
}
```

#### Campos marcados como `[Obsolete]`
```csharp
[Obsolete("Yape ya no se usa para clientes. Solo efectivo.")]
public string? NumeroYape { get; set; }

[Obsolete("Plin ya no se usa para clientes. Solo efectivo.")]
public string? NumeroPlin { get; set; }
```

---

## 📋 FASE 3: Endpoints para Operadores

### Objetivo
Crear endpoints para que operadores gestionen las pre-reservas pendientes.

### 🔧 3.1 Confirmar Reserva

#### Endpoint
```
POST /api/Reserva/confirmar-reserva-operador
```

#### DTO de Entrada: `ConfirmarReservaOperadorDto`
```csharp
public class ConfirmarReservaOperadorDto
{
    public int IdReserva { get; set; }
    public decimal? MontoAdelanto { get; set; }     // Opcional: 0 = sin adelanto
    public string? NumeroRecibo { get; set; }       // Número de recibo/voucher
    public string? ObservacionOperador { get; set; } // Notas internas
}
```

#### Flujo del Handler: `ConfirmarReservaOperadorCommandHandler`

1. **Validar que la reserva existe y está PENDIENTE**
```csharp
var reserva = await _ReservaRepository.GetByAsync(
    r => r.IdReserva == request.ConfirmarDto.IdReserva && r.Activo
);

if (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente)
{
    response.AddErrorResult("La reserva no puede ser confirmada.");
    return response;
}
```

2. **Validar que NO ha expirado**
```csharp
if (reserva.FechaExpiracionPreReserva.HasValue
    && reserva.FechaExpiracionPreReserva.Value < DateTimeOffset.Now)
{
    response.AddErrorResult("La reserva ha expirado.");
    return response;
}
```

3. **Validar porcentaje mínimo de adelanto**
```csharp
decimal porcentajeMinimoAdelanto = cancha?.PorcentajeAdelanto ?? 50;

if (montoAdelanto > 0)
{
    decimal porcentajeAdelanto = (montoAdelanto / montoTotal) * 100;
    if (porcentajeAdelanto < porcentajeMinimoAdelanto)
    {
        response.AddErrorResult(
            $"El adelanto debe ser al menos {porcentajeMinimoAdelanto}% del total."
        );
        return response;
    }
}
```

4. **Actualizar el pago**
```csharp
pago.MontoAdelanto = montoAdelanto;
pago.MontoPendiente = montoTotal - montoAdelanto;
pago.NumeroReferencia = request.ConfirmarDto.NumeroRecibo;

// Determinar estado del pago
if (montoAdelanto >= montoTotal)
    pago.IdEstadoPago = estadoPagado.IdEstadoPago;
else if (montoAdelanto > 0)
    pago.IdEstadoPago = estadoParcial.IdEstadoPago;
else
    pago.IdEstadoPago = estadoPendiente.IdEstadoPago;
```

5. **Cambiar estado a CONFIRMADO**
```csharp
reserva.IdEstadoReserva = estadoConfirmado.IdEstadoReserva;
reserva.FechaExpiracionPreReserva = null; // Limpiar expiración
```

6. **Notificar al cliente**
```csharp
await _notificacionService.NotificarReservaConfirmadaAsync(
    reserva, cancha, cliente, pago
);
```

#### Validaciones del Validator
```csharp
public class ConfirmarReservaOperadorCommandValidator : AbstractValidator<ConfirmarReservaOperadorCommand>
{
    public ConfirmarReservaOperadorCommandValidator()
    {
        RuleFor(x => x.ConfirmarDto.IdReserva)
            .GreaterThan(0)
            .WithMessage("El ID de la reserva es requerido.");

        RuleFor(x => x.ConfirmarDto.MontoAdelanto)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ConfirmarDto.MontoAdelanto.HasValue)
            .WithMessage("El monto de adelanto no puede ser negativo.");
    }
}
```

---

### 🔧 3.2 Liberar/Cancelar Reserva

#### Endpoint
```
POST /api/Reserva/liberar-reserva-operador
```

#### DTO de Entrada: `LiberarReservaOperadorDto`
```csharp
public class LiberarReservaOperadorDto
{
    public int IdReserva { get; set; }
    public string Motivo { get; set; } = null!; // Razón de cancelación
}
```

#### Flujo del Handler: `LiberarReservaOperadorCommandHandler`

1. **Validar reserva PENDIENTE**
2. **Cambiar estado a CANCELADO**
```csharp
reserva.IdEstadoReserva = estadoCancelado.IdEstadoReserva;
reserva.FechaExpiracionPreReserva = null;
```
3. **Notificar al cliente**
```csharp
await _notificacionService.NotificarReservaCanceladaAsync(
    reserva, cliente, request.LiberarDto.Motivo
);
```

---

### 🔧 3.3 Listar Reservas Pendientes del Proveedor

#### Endpoint
```
GET /api/Reserva/pendientes-operador/{idProveedor}
```

#### DTO de Respuesta: `ReservaPendienteOperadorDto`
```csharp
public class ReservaPendienteOperadorDto
{
    public int IdReserva { get; set; }
    public string CodigoReserva { get; set; }

    // Información del cliente
    public string NombreCliente { get; set; }
    public string EmailCliente { get; set; }
    public string TelefonoCliente { get; set; }

    // Información de la reserva
    public string NombreCancha { get; set; }
    public DateTimeOffset Fecha { get; set; }
    public decimal Monto { get; set; }
    public List<HorarioDto> Horarios { get; set; }

    // Control de expiración
    public DateTimeOffset FechaExpiracion { get; set; }
    public double HorasRestantes { get; set; }
    public string NivelUrgencia { get; set; } // "CRÍTICA", "ALTA", "MEDIA", "BAJA"

    public DateTimeOffset FechaCreacion { get; set; }
}
```

#### Cálculo del Nivel de Urgencia
```csharp
private string CalcularNivelUrgencia(DateTimeOffset? fechaExpiracion)
{
    if (!fechaExpiracion.HasValue) return "BAJA";

    var horasRestantes = (fechaExpiracion.Value - DateTimeOffset.Now).TotalHours;

    if (horasRestantes <= 6) return "CRÍTICA";      // ⚠️ Menos de 6 horas
    if (horasRestantes <= 24) return "ALTA";        // 🔴 Menos de 24 horas
    if (horasRestantes <= 48) return "MEDIA";       // 🟠 Menos de 48 horas
    return "BAJA";                                  // 🟢 Más de 48 horas
}
```

#### Ordenamiento
```csharp
.OrderBy(r => r.FechaExpiracionPreReserva) // Más urgentes primero
```

---

## 📋 FASE 4: Sistema de Notificaciones

### Objetivo
Implementar notificaciones duales (Email + WhatsApp) en todos los flujos críticos.

### 🔔 4.1 Servicio de Email (Ya existente)

Usa MimeKit y SendEmailCommand (MediatR).

**Templates HTML profesionales** con:
- Estilos inline para compatibilidad
- Información estructurada
- Call-to-action claro

---

### 🔔 4.2 WhatsApp Cloud API - NUEVO

#### Servicio: `WhatsAppService.cs`

**Interfaz:**
```csharp
public interface IWhatsAppService
{
    Task<bool> SendTextMessageAsync(string phoneNumber, string message);
    Task<int> SendBulkTextMessageAsync(List<string> phoneNumbers, string message);
}
```

**Características:**
- ✅ Limpieza automática de números (agrega +51 si falta)
- ✅ Rate limiting (500ms entre mensajes)
- ✅ Logging completo de éxitos/errores
- ✅ Flag para habilitar/deshabilitar

**Configuración en appsettings.json:**
```json
"WhatsApp": {
  "Enabled": false,                      // Cambiar a true en producción
  "PhoneNumberId": "XXXXXXXXXXXXXXX",    // Desde Meta for Developers
  "AccessToken": "EAAXXX...",            // Token permanente
  "ApiVersion": "v18.0"
}
```

**Integración con Facebook Graph API:**
```csharp
POST https://graph.facebook.com/v18.0/{PhoneNumberId}/messages
Authorization: Bearer {AccessToken}
Content-Type: application/json

{
  "messaging_product": "whatsapp",
  "recipient_type": "individual",
  "to": "+51987654321",
  "type": "text",
  "text": {
    "preview_url": false,
    "body": "Mensaje aquí..."
  }
}
```

---

### 🔔 4.3 Servicio Unificado de Notificaciones

#### Interfaz: `INotificacionService`
```csharp
public interface INotificacionService
{
    // Cliente → Operadores
    Task NotificarNuevaReservaPendienteAsync(
        Reserva reserva, Cancha cancha, AspNetUsers cliente, List<Operador> operadores);

    // Operador → Cliente
    Task NotificarReservaConfirmadaAsync(
        Reserva reserva, Cancha cancha, AspNetUsers cliente, Pago pago);

    // Sistema → Operadores (Advertencia)
    Task NotificarReservaProximaExpirarAsync(
        Reserva reserva, Cancha cancha, AspNetUsers cliente, List<Operador> operadores);

    // Sistema → Operadores (Expiración)
    Task NotificarReservaExpiradaAsync(
        Reserva reserva, Cancha cancha, List<Operador> operadores);

    // Operador → Cliente (Cancelación)
    Task NotificarReservaCanceladaAsync(
        Reserva reserva, AspNetUsers cliente, string motivo);
}
```

#### Ejemplo de Mensaje WhatsApp:
```
🔔 *Nueva Reserva Pendiente*

📋 Código: *RES-2025-0001*
⚽ Cancha: Cancha de Fútbol 7
📅 Fecha: 15/01/2025
💰 Monto: S/ 80.00

👤 *Cliente:*
Nombre: Juan Pérez
Teléfono: +51987654321
Email: juan@example.com

⏰ Expira: 15/01/2025 14:30

Por favor, contacta al cliente para coordinar el pago.
```

---

## 📋 FASE 5: Background Services y Endpoint de Cliente

### Objetivo
Automatizar expiración de reservas y permitir que clientes vean su historial.

### ⏰ 5.1 Background Service: `ReservaExpirationService`

**Hereda de:** `BackgroundService` (IHostedService)

**Frecuencia:** Cada 30 minutos

**Tareas:**

#### 1. Procesar Reservas Expiradas
```csharp
private async Task ProcesarReservasExpiradas(CancellationToken cancellationToken)
{
    // 1. Buscar reservas PENDIENTES con fechaExpiracion <= NOW
    var reservasExpiradas = await _reservaRepository.FindByAsync(
        r => r.Activo
             && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
             && r.FechaExpiracionPreReserva.HasValue
             && r.FechaExpiracionPreReserva.Value <= DateTimeOffset.Now
    );

    // 2. Cambiar estado a EXPIRADO
    foreach (var reserva in reservasExpiradas)
    {
        reserva.IdEstadoReserva = estadoExpirado.IdEstadoReserva;
        await _reservaRepository.UpdateAsync(reserva);

        // 3. Notificar a operadores
        await _notificacionService.NotificarReservaExpiradaAsync(...);
    }
}
```

#### 2. Notificar Reservas Próximas a Expirar
```csharp
private async Task NotificarReservasProximasExpirar(CancellationToken cancellationToken)
{
    var ahora = DateTimeOffset.Now;
    var limiteAdvertencia = ahora.AddHours(6); // 6 horas antes

    // Buscar reservas PENDIENTES que expiran en menos de 6 horas
    // Y que NO hayan sido notificadas aún (notificacionAdvertenciaEnviada = false)
    var reservasProximasExpirar = await _reservaRepository.FindByAsync(
        r => r.Activo
             && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
             && r.FechaExpiracionPreReserva.HasValue
             && r.FechaExpiracionPreReserva.Value > ahora
             && r.FechaExpiracionPreReserva.Value <= limiteAdvertencia
             && !r.NotificacionAdvertenciaEnviada
    );

    foreach (var reserva in reservasProximasExpirar)
    {
        // Enviar advertencia
        await _notificacionService.NotificarReservaProximaExpirarAsync(...);

        // Marcar como notificada para evitar duplicados
        reserva.NotificacionAdvertenciaEnviada = true;
        await _reservaRepository.UpdateAsync(reserva);
    }
}
```

**Registro del servicio:**
```csharp
services.AddHostedService<ReservaExpirationService>();
```

---

### 👤 5.2 Endpoint para Cliente: Ver Mis Reservas

#### Endpoint
```
GET /api/Reserva/mis-reservas/{idUsuario}
```

#### DTO de Respuesta: `ReservaClienteDto`
```csharp
public class ReservaClienteDto
{
    public int IdReserva { get; set; }
    public string CodigoReserva { get; set; }
    public DateTimeOffset Fecha { get; set; }
    public decimal Monto { get; set; }

    // Estado
    public string EstadoReserva { get; set; }        // "Pendiente", "Confirmado", etc.
    public string CodigoEstadoReserva { get; set; }  // "01", "02", "03", "04"

    // Cancha
    public int IdCancha { get; set; }
    public string NombreCancha { get; set; }
    public string DireccionCancha { get; set; }
    public string? TelefonoCancha { get; set; }

    // Horarios
    public List<HorarioReservadoDto> Horarios { get; set; }

    // Pago
    public string EstadoPago { get; set; }      // "Pagado", "Parcial", "Pendiente"
    public decimal MontoAdelanto { get; set; }
    public decimal MontoPendiente { get; set; }
    public string? NumeroRecibo { get; set; }

    // Fechas
    public DateTimeOffset? FechaExpiracionPreReserva { get; set; }
    public DateTimeOffset FechaCreacion { get; set; }

    // Propiedades calculadas para UI
    public bool EstaConfirmada => CodigoEstadoReserva == "02";
    public bool EstaPendiente => CodigoEstadoReserva == "01";
    public bool EstaCancelada => CodigoEstadoReserva == "03";
    public bool EstaExpirada => CodigoEstadoReserva == "04";
    public bool TienePagoPendiente => MontoPendiente > 0;
}

public class HorarioReservadoDto
{
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string HorarioFormateado => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
}
```

#### Query Handler: `ReservasClienteQueryHandler`
```csharp
var reservas = await _reservaRepository.FindByAsNoTrackingAsync(
    r => r.IdUsuario == request.IdUsuario && r.Activo,
    r => r.IdCanchaNavigation,
    r => r.IdEstadoReservaNavigation,
    r => r.ReservaDetalle,
    r => r.Pago
);

var reservasDtos = reservas
    .OrderByDescending(r => r.Fecha)           // Más recientes primero
    .ThenByDescending(r => r.CreateDate)
    .Select(r => new ReservaClienteDto { ... })
    .ToList();
```

---

## 🌊 FLUJOS COMPLETOS DEL SISTEMA

### 🎯 Flujo 1: Cliente Crea Pre-Reserva

```
┌─────────────┐
│   CLIENTE   │
│ (Frontend)  │
└──────┬──────┘
       │
       │ 1. POST /api/Reserva
       │    {
       │      idCancha: 1,
       │      idUsuario: "guid",
       │      fecha: "2025-01-15",
       │      detalles: [
       │        { horaInicio: "18:00", horaFin: "19:00" }
       │      ],
       │      monto: 80,
       │      codigoMetodoPago: "EFE"
       │    }
       ▼
┌──────────────────────────┐
│  CreateReservaHandler    │
├──────────────────────────┤
│ 1. Validar cancha existe │
│ 2. Solo acepta EFECTIVO  │
│ 3. Validar horario libre │
│ 4. Crear PENDIENTE       │
│ 5. Generar código único  │
│ 6. Calcular expiración   │
│ 7. Crear pago PENDIENTE  │
│ 8. Obtener operadores    │
└──────────┬───────────────┘
           │
           ├─────────────────────────────┐
           │                             │
           ▼                             ▼
    ┌─────────────┐            ┌──────────────────┐
    │    EMAIL    │            │     WHATSAPP     │
    │ Operadores  │            │    Operadores    │
    └─────────────┘            └──────────────────┘

           Respuesta al Cliente:
           {
             reserva: { ... },
             pago: { ... },
             codigoReserva: "RES-2025-0001",
             telefonoCancha: "+51987654321",
             nombreOperador: "Juan Operador",
             informacionAdicional: "Tu reserva expira en 48 horas..."
           }
```

---

### 🎯 Flujo 2: Operador Confirma Reserva

```
┌──────────────┐
│  OPERADOR    │
│  (Dashboard) │
└──────┬───────┘
       │
       │ 1. GET /api/Reserva/pendientes-operador/{idProveedor}
       │    → Lista de reservas con nivel de urgencia
       │
       │ 2. Contacta al cliente por teléfono/WhatsApp
       │    (Fuera del sistema)
       │
       │ 3. Cliente paga en efectivo
       │    (Fuera del sistema)
       │
       │ 4. POST /api/Reserva/confirmar-reserva-operador
       │    {
       │      idReserva: 123,
       │      montoAdelanto: 40,  // 50% del total
       │      numeroRecibo: "REC-001",
       │      observacionOperador: "Cliente pagó en efectivo"
       │    }
       ▼
┌──────────────────────────────────┐
│ ConfirmarReservaOperadorHandler  │
├──────────────────────────────────┤
│ 1. Validar PENDIENTE             │
│ 2. Validar NO expirado           │
│ 3. Validar % mínimo adelanto     │
│ 4. Actualizar pago               │
│    - MontoAdelanto: 40           │
│    - MontoPendiente: 40          │
│    - Estado: PARCIAL             │
│ 5. Cambiar estado a CONFIRMADO   │
│ 6. Limpiar fechaExpiracion       │
└──────────┬───────────────────────┘
           │
           ├─────────────────────────────┐
           │                             │
           ▼                             ▼
    ┌─────────────┐            ┌──────────────────┐
    │    EMAIL    │            │     WHATSAPP     │
    │   Cliente   │            │      Cliente     │
    └─────────────┘            └──────────────────┘

           Contenido de notificación:
           - Reserva confirmada ✅
           - Código: RES-2025-0001
           - Fecha y horarios
           - Adelanto pagado: S/ 40
           - Pendiente: S/ 40
           - Dirección y teléfono cancha
```

---

### 🎯 Flujo 3: Reserva Expira Automáticamente

```
┌─────────────────────────┐
│ ReservaExpirationService│  ⏰ Cada 30 minutos
│  (Background Service)   │
└──────────┬──────────────┘
           │
           │ 1. Buscar PENDIENTES con fechaExpiracion <= NOW
           ▼
┌──────────────────────────────┐
│ ¿Encontró reservas expiradas?│
└───────┬──────────────────────┘
        │
        │ SÍ
        ▼
┌──────────────────────────────┐
│ Para cada reserva:           │
│ 1. Cambiar a EXPIRADO        │
│ 2. Obtener operadores        │
│ 3. Enviar notificación       │
└──────────┬───────────────────┘
           │
           ├─────────────────────────────┐
           │                             │
           ▼                             ▼
    ┌─────────────┐            ┌──────────────────┐
    │    EMAIL    │            │     WHATSAPP     │
    │ Operadores  │            │    Operadores    │
    └─────────────┘            └──────────────────┘

           Contenido:
           - Reserva RES-2025-0001 ha expirado
           - Cliente: Juan Pérez
           - Fecha: 15/01/2025 18:00-19:00
           - El horario está disponible nuevamente
```

---

### 🎯 Flujo 4: Advertencia de Proximidad

```
┌─────────────────────────┐
│ ReservaExpirationService│  ⏰ Cada 30 minutos
│  (Background Service)   │
└──────────┬──────────────┘
           │
           │ 1. Buscar PENDIENTES que expiran en < 6 horas
           │    Y notificacionAdvertenciaEnviada = false
           ▼
┌──────────────────────────────────┐
│ ¿Encontró reservas por vencer?   │
└───────┬──────────────────────────┘
        │
        │ SÍ
        ▼
┌──────────────────────────────────┐
│ Para cada reserva:               │
│ 1. Enviar advertencia            │
│ 2. Marcar como notificada        │
│    (notificacionAdvertenciaEnviada = true) │
└──────────┬───────────────────────┘
           │
           ├─────────────────────────────┐
           │                             │
           ▼                             ▼
    ┌─────────────┐            ┌──────────────────┐
    │    EMAIL    │            │     WHATSAPP     │
    │ Operadores  │            │    Operadores    │
    │ + Cliente   │            │   + Cliente      │
    └─────────────┘            └──────────────────┘

           Contenido:
           - ⚠️ Reserva RES-2025-0001 expira pronto
           - Quedan 4 horas
           - Cliente: Juan Pérez (+51987654321)
           - Contactar urgentemente
```

---

### 🎯 Flujo 5: Cliente Ve Sus Reservas

```
┌─────────────┐
│   CLIENTE   │
│ (Frontend)  │
└──────┬──────┘
       │
       │ GET /api/Reserva/mis-reservas/{idUsuario}
       ▼
┌──────────────────────────┐
│ ReservasClienteHandler   │
├──────────────────────────┤
│ 1. Buscar todas activas  │
│ 2. Incluir navegaciones  │
│ 3. Ordenar desc por fecha│
│ 4. Mapear a DTOs         │
└──────────┬───────────────┘
           │
           │ Respuesta:
           │ [
           │   {
           │     codigoReserva: "RES-2025-0001",
           │     estadoReserva: "Confirmado",
           │     nombreCancha: "Cancha Fútbol 7",
           │     fecha: "2025-01-15",
           │     horarios: [{ horaInicio: "18:00", horaFin: "19:00" }],
           │     monto: 80,
           │     estadoPago: "Parcial",
           │     montoAdelanto: 40,
           │     montoPendiente: 40,
           │     estaConfirmada: true,
           │     tienePagoPendiente: true
           │   },
           │   { ... más reservas ... }
           │ ]
           ▼
    ┌──────────────┐
    │  Frontend    │
    │  Renderiza   │
    │  Historial   │
    └──────────────┘
```

---

## 📊 MATRIZ DE ENDPOINTS

### Para CLIENTES

| Método | Endpoint | Descripción | Request | Response |
|--------|----------|-------------|---------|----------|
| POST | `/api/Reserva` | Crear pre-reserva | `CreateReservaDto` | `ReservaConPagoDto` |
| GET | `/api/Reserva/mis-reservas/{idUsuario}` | Ver historial completo | - | `List<ReservaClienteDto>` |
| GET | `/api/Reserva/{id}` | Ver detalle de 1 reserva | - | `GetReservaDto` |

### Para OPERADORES

| Método | Endpoint | Descripción | Request | Response |
|--------|----------|-------------|---------|----------|
| GET | `/api/Reserva/pendientes-operador/{idProveedor}` | Listar pendientes con urgencia | - | `List<ReservaPendienteOperadorDto>` |
| POST | `/api/Reserva/confirmar-reserva-operador` | Confirmar y registrar pago | `ConfirmarReservaOperadorDto` | `GetReservaDto` |
| POST | `/api/Reserva/liberar-reserva-operador` | Cancelar/liberar pendiente | `LiberarReservaOperadorDto` | `GetReservaDto` |

---

## 🎨 IDEAS ADICIONALES PARA EL FRONTEND CLIENTE

### 1. **Pantalla: Crear Reserva** (Ya existe, modificar)
- ✅ Mostrar horarios disponibles del día
- ✅ Seleccionar horarios
- ✅ Ver precio total
- ✅ **NUEVO:** Mostrar mensaje de pre-reserva antes de confirmar
  ```
  ⚠️ Tu reserva será PRE-RESERVADA por 48 horas.
  El operador de la cancha se contactará contigo para
  coordinar el pago en efectivo.
  ```
- ✅ **NUEVO:** Después de crear, mostrar:
  - Código de reserva: **RES-2025-0001**
  - Fecha de expiración: 15/01/2025 14:30
  - Teléfono del operador: +51987654321
  - Mensaje: "Espera la llamada del operador"

---

### 2. **Pantalla NUEVA: Mis Reservas / Historial**

#### Vista de Lista
```
┌─────────────────────────────────────────────┐
│  MIS RESERVAS                    [Filtros ▼]│
├─────────────────────────────────────────────┤
│                                             │
│  ⏳ PENDIENTE - RES-2025-0001              │
│  Cancha Fútbol 7 - Estadio Central          │
│  📅 15 Enero 2025, 18:00 - 19:00            │
│  💰 S/ 80.00                                 │
│  ⏰ Expira en 23 horas                       │
│  ────────────────────────────────────────── │
│  [Ver Detalles]  [Contactar Operador]      │
│                                             │
├─────────────────────────────────────────────┤
│                                             │
│  ✅ CONFIRMADA - RES-2025-0002             │
│  Cancha Tenis - Club Deportivo              │
│  📅 20 Enero 2025, 10:00 - 11:00            │
│  💰 S/ 60.00 (Adelanto: S/ 30 | Pend: S/ 30)│
│  ────────────────────────────────────────── │
│  [Ver Detalles]  [Recibo]                  │
│                                             │
├─────────────────────────────────────────────┤
│                                             │
│  ❌ EXPIRADA - RES-2025-0003               │
│  Cancha Vóley - Centro Deportivo            │
│  📅 10 Enero 2025, 16:00 - 17:00            │
│  💰 S/ 50.00                                 │
│  ────────────────────────────────────────── │
│  [Ver Detalles]  [Reservar Nuevamente]     │
│                                             │
└─────────────────────────────────────────────┘
```

#### Filtros
- Todas
- Pendientes
- Confirmadas
- Canceladas / Expiradas

#### Vista de Detalle
```
┌─────────────────────────────────────────────┐
│  ← Volver                                   │
├─────────────────────────────────────────────┤
│  RESERVA: RES-2025-0001                     │
│  Estado: ⏳ PENDIENTE                       │
├─────────────────────────────────────────────┤
│                                             │
│  📍 CANCHA                                   │
│  Nombre: Cancha Fútbol 7                    │
│  Dirección: Av. Principal 123                │
│  Teléfono: +51987654321                      │
│                                             │
│  📅 FECHA Y HORARIO                          │
│  15 de Enero de 2025                         │
│  18:00 - 19:00 (1 hora)                      │
│                                             │
│  💰 PAGO                                     │
│  Monto Total: S/ 80.00                       │
│  Estado: PENDIENTE                           │
│  Método: Efectivo                            │
│                                             │
│  ⏰ IMPORTANTE                                │
│  Expira: 15/01/2025 14:30 (23 horas)        │
│  El operador te contactará pronto            │
│                                             │
│  ────────────────────────────────────────── │
│                                             │
│  [📞 Contactar Operador]                    │
│  [📍 Ver Ubicación en Mapa]                 │
│  [❌ Cancelar Reserva]                      │
│                                             │
└─────────────────────────────────────────────┘
```

---

### 3. **Notificaciones In-App** (Opcional)

Usar **SignalR** o **polling** para notificaciones en tiempo real:

```
┌─────────────────────────────────────────────┐
│  🔔 Notificaciones                    [1]   │
├─────────────────────────────────────────────┤
│  ✅ ¡Tu reserva RES-2025-0001 fue           │
│     confirmada!                      hace 5m│
│  [Ver detalles]                             │
└─────────────────────────────────────────────┘
```

---

### 4. **Perfil del Usuario**

```
┌─────────────────────────────────────────────┐
│  MI PERFIL                                  │
├─────────────────────────────────────────────┤
│                                             │
│  📸 [Foto de Perfil]                        │
│                                             │
│  👤 Información Personal                     │
│  Nombre: Juan Pérez                          │
│  Email: juan@example.com                     │
│  Teléfono: +51987654321                      │
│  [Editar]                                   │
│                                             │
│  📊 Estadísticas                             │
│  Reservas totales: 15                        │
│  Reservas confirmadas: 12                    │
│  Reservas canceladas: 2                      │
│  Reservas expiradas: 1                       │
│                                             │
│  ⚙️ Configuración                            │
│  [ ] Recibir notificaciones por email       │
│  [ ] Recibir notificaciones por WhatsApp    │
│                                             │
│  🔐 Seguridad                                │
│  [Cambiar Contraseña]                       │
│  [Cerrar Sesión]                            │
│                                             │
└─────────────────────────────────────────────┘
```

---

### 5. **Página: Buscar Canchas** (Ya existe, mejorar)

Agregar indicadores de disponibilidad:

```
┌─────────────────────────────────────────────┐
│  CANCHA FÚTBOL 7                            │
│  📍 Av. Principal 123                       │
│  ⭐⭐⭐⭐⭐ 4.8 (120 reseñas)               │
│  💰 S/ 80 por hora                          │
│  ✅ Disponible hoy: 18:00, 19:00, 20:00    │
│  [Ver más] [Reservar]                       │
└─────────────────────────────────────────────┘
```

---

### 6. **Modal: Confirmar Reserva** (Nuevo)

Antes de enviar la reserva:

```
┌─────────────────────────────────────────────┐
│  ⚠️ CONFIRMAR PRE-RESERVA                   │
├─────────────────────────────────────────────┤
│                                             │
│  Estás por crear una PRE-RESERVA:           │
│                                             │
│  ✓ Cancha Fútbol 7                          │
│  ✓ 15 de Enero, 18:00 - 19:00               │
│  ✓ Total: S/ 80.00                          │
│                                             │
│  ⚠️ Importante:                              │
│  • Tu reserva estará PENDIENTE por 48 horas │
│  • El operador te contactará para coordinar │
│  • Debes pagar en EFECTIVO                  │
│  • Si no confirmas a tiempo, expirará       │
│                                             │
│  ────────────────────────────────────────── │
│                                             │
│  [Cancelar]  [Confirmar Pre-Reserva]       │
│                                             │
└─────────────────────────────────────────────┘
```

---

## 🎯 ROADMAP DE IMPLEMENTACIÓN EN ANGULAR

### Sprint 1: Modificar Flujo de Reserva Existente
1. Modificar `payment.component.ts/html`
   - Remover opciones de Yape/Plin/Transferencia
   - Solo mostrar "Efectivo"
   - Mostrar mensaje de pre-reserva
   - Mostrar modal de confirmación

2. Modificar respuesta después de crear reserva
   - Mostrar código de reserva
   - Mostrar fecha de expiración
   - Mostrar teléfono del operador
   - Botón "Contactar Operador" (abre WhatsApp)

### Sprint 2: Crear Módulo "Mis Reservas"
1. Crear servicio `reserva.service.ts`
   - Método `getMisReservas(idUsuario)`
   - Método `getDetalleReserva(idReserva)`

2. Crear componente `mis-reservas-list.component`
   - Lista con filtros
   - Badges de estado (pendiente, confirmada, expirada)
   - Indicador de tiempo restante

3. Crear componente `reserva-detalle.component`
   - Vista completa de la reserva
   - Información de pago
   - Botones de acción

### Sprint 3: Perfil de Usuario
1. Crear componente `perfil.component`
   - Ver información personal
   - Editar datos
   - Estadísticas de reservas
   - Configuración de notificaciones

### Sprint 4: Mejoras y Pulido
1. Notificaciones in-app (opcional con SignalR)
2. Animaciones y transiciones
3. Manejo de errores mejorado
4. Loading states
5. Responsive design

---

## 📝 RESUMEN PARA FRONTEND

### Endpoints que necesitarás consumir:

#### Cliente
1. `POST /api/Reserva` - Crear pre-reserva
2. `GET /api/Reserva/mis-reservas/{idUsuario}` - Listar historial
3. `GET /api/Reserva/{id}` - Ver detalle

#### Autenticación (ya existente)
- JWT Bearer Token en headers
- Obtener `idUsuario` del token decodificado

### Estados de Reserva a Manejar:
- **01 - PENDIENTE** 🟡: Mostrar countdown, botón contactar operador
- **02 - CONFIRMADO** 🟢: Mostrar info de pago, botón ver recibo
- **03 - CANCELADO** ⚫: Opción de reservar nuevamente
- **04 - EXPIRADO** 🔴: Opción de reservar nuevamente

### Estados de Pago a Manejar:
- **PAGADO** ✅: Todo pagado
- **PARCIAL** ⏳: Mostrar adelanto y pendiente
- **PENDIENTE** ⏰: Coordinando con operador

---

¿Todo claro? ¿Empezamos con el Sprint 1 modificando el flujo de reserva existente en Angular? 🚀
