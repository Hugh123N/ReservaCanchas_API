# 📋 FLUJO DE PAGO CON CULQI

## 📌 Tabla de Contenidos
- [Introducción](#introducción)
- [Contexto de Uso](#contexto-de-uso)
- [Arquitectura General](#arquitectura-general)
- [Métodos de Pago Soportados](#métodos-de-pago-soportados)
- [Flujo de Pagos Únicos (Charges)](#flujo-de-pagos-únicos-charges)
- [Flujo de Suscripciones (Planes SaaS)](#flujo-de-suscripciones-planes-saas)
- [Componentes Implementados](#componentes-implementados)
- [Configuración](#configuración)
- [Webhooks y Eventos](#webhooks-y-eventos)
- [Limitaciones y Consideraciones](#limitaciones-y-consideraciones)
- [Troubleshooting](#troubleshooting)

---

## 🎯 Introducción

Este documento describe la implementación completa de la integración con **Culqi**, la pasarela de pagos líder en Perú, para procesar pagos de **planes de proveedores** y **suscripciones recurrentes**.

### ¿Por qué Culqi?

✅ **Acepta persona natural** - No requiere ser empresa con RUC
✅ **Comisiones competitivas** - 3.44% + S/ 0.20 por transacción
✅ **Métodos populares** - Yape, Plin, tarjetas, billeteras móviles
✅ **API bien documentada** - Integración sencilla y segura
✅ **Webhooks confiables** - Notificaciones automáticas de pagos
✅ **Suscripciones nativas** - Soporte para cobros recurrentes

---

## 🏢 Contexto de Uso

### ⚠️ IMPORTANTE: Separación de Responsabilidades

Esta integración de Culqi está diseñada **EXCLUSIVAMENTE para pagos de planes de proveedores**, NO para reservas de canchas.

| Concepto | Método de Pago | Sistema |
|----------|----------------|---------|
| **Reservas de Canchas** | EFECTIVO únicamente | Manual (operador confirma) |
| **Planes de Proveedores** | Yape/Plin/Tarjetas vía Culqi | Automático (webhook confirma) |

### ¿Qué son los Planes de Proveedores?

Los proveedores de canchas pueden contratar planes/paquetes para:
- Destacar sus canchas en la plataforma
- Acceder a funcionalidades premium
- Aumentar su visibilidad
- Obtener reportes avanzados

Estos planes se pagan mediante Culqi con confirmación automática y **renovación recurrente**.

---

## 🏗️ Arquitectura General

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND (React/Vue/etc)                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  1. Usuario selecciona plan y método de pago            │   │
│  │  2. Frontend carga CulqiJS desde CDN                     │   │
│  │  3. CulqiJS captura datos y crea TOKEN                   │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ TOKEN (encriptado)
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      BACKEND (.NET API)                         │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  4. API recibe token del frontend                        │   │
│  │  5. CulqiService crea CUSTOMER (si es nuevo)             │   │
│  │  6. CulqiService crea PLAN en Culqi                      │   │
│  │  7. CulqiService crea SUBSCRIPTION (pago recurrente)    │   │
│  │  8. Guarda ProveedorPlan con estado PENDIENTE            │   │
│  │  9. Retorna respuesta al frontend                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ Crear Suscripción
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                        CULQI API                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  10. Culqi procesa el pago inicial                      │   │
│  │  11. Usuario completa pago (Yape/Plin/Tarjeta)           │   │
│  │  12. Culqi confirma transacción                          │   │
│  │  13. Culqi programa próximos cobros automáticos          │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ WEBHOOK (async)
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│              WEBHOOK ENDPOINT (CulqiWebhookController)          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  14. Culqi envía notificación (charge/subscription)     │   │
│  │  15. Backend valida webhook                              │   │
│  │  16. Actualiza ProveedorPlan a ACTIVO                    │   │
│  │  17. Notifica al proveedor por email                     │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 💳 Métodos de Pago Soportados

### 1. **Yape** 🟡

- **Límite**: Máximo S/ 2,000 por transacción
- **Moneda**: Solo soles (PEN)
- **Validez**: Código de aprobación válido por 2 minutos
- **Flujo**: Usuario ingresa número Yape y código de aprobación

### 2. **Plin** 🔵

- **Límite**: Según configuración de Culqi
- **Moneda**: Soles (PEN)
- **Flujo**: QR code que usuario escanea con app Plin

### 3. **Tarjetas de Crédito/Débito** 💳

- **Marcas**: Visa, Mastercard, American Express, Diners
- **Moneda**: PEN (soles)
- **Seguridad**: Formulario PCI-compliant de Culqi
- **Suscripciones**: Se guarda la tarjeta para cobros recurrentes

### 4. **Billeteras Móviles** 📱

- **Opciones**: Otras billeteras digitales disponibles en Culqi
- **Flujo**: QR code

---

## 🔄 Flujo de Pagos Únicos (Charges)

### ¿Cuándo usar?
Para pagos únicos que NO son parte de una suscripción. Ejemplo: pagos especiales, servicios adicionales, etc.

### FASE 1: Frontend - Generación de Token

```javascript
// 1. Cargar CulqiJS en tu HTML
<script src="https://checkout.culqi.com/js/v4"></script>

// 2. Configurar Culqi con tu clave pública
Culqi.publicKey = 'pk_test_XXXXXXXXXXXXXXXX';

// 3. Configurar opciones
Culqi.settings({
  title: 'Pago Único',
  currency: 'PEN',
  amount: 10000  // En centavos (S/ 100.00)
});

// 4. Abrir formulario
Culqi.open();

// 5. Capturar el token
function culqi() {
  if (Culqi.token) {
    const token = Culqi.token.id;
    // Enviar al backend...
  }
}
```

### FASE 2: Backend - Creación del Cargo

```csharp
// Crear request para Culqi
var culqiRequest = new CulqiCreateChargeRequest
{
    Amount = CulqiService.ConvertToCents(monto),
    CurrencyCode = "PEN",
    Email = dto.Email,
    SourceId = dto.CulqiToken,
    Description = "Descripción del pago",
    Metadata = new Dictionary<string, string>
    {
        { "tipo", "pago_unico" }
    }
};

var culqiResponse = await _culqiService.CreateChargeAsync(culqiRequest);
```

---

## 🔄 Flujo de Suscripciones (Planes SaaS)

### ¿Cuándo usar?
Para **planes de proveedores** con cobros recurrentes automáticos (mensual, anual, etc.).

### FASE 1: Frontend - Generación de Token

```javascript
// 1. Cargar CulqiJS
<script src="https://checkout.culqi.com/js/v4"></script>

// 2. Configurar
Culqi.publicKey = 'pk_test_XXXXXXXXXXXXXXXX';

Culqi.settings({
  title: 'Suscripción Plan Premium',
  currency: 'PEN',
  description: 'Plan Premium - Renovación automática',
  amount: 9900  // S/ 99.00 en centavos
});

// 3. Abrir checkout
Culqi.open();

// 4. Enviar token al backend
function culqi() {
  if (Culqi.token) {
    fetch('/api/proveedor-plan/checkout', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        idProveedor: 1,
        idPlane: 2,
        idPlanTarifa: 3,
        culqiToken: Culqi.token.id,
        email: 'proveedor@example.com'
      })
    });
  }
}
```

### FASE 2: Backend - Creación de Suscripción

El flujo en `CheckoutPlanCommandHandler` sigue estos pasos:

#### Paso 1: Crear o recuperar Customer en Culqi

```csharp
// Si el proveedor no tiene CulqiCustomerId, crearlo
var customerId = proveedor.CulqiCustomerId;
if (string.IsNullOrEmpty(customerId))
{
    var customerRequest = new CulqiCreateCustomerRequest
    {
        Email = dto.Email,
        Code = $"prov_{proveedor.IdProveedor}",
        FirstName = proveedor.IdUsuarioNavigation?.Nombres,
        LastName = proveedor.IdUsuarioNavigation?.Apellidos,
        Metadata = new Dictionary<string, string>
        {
            { "proveedor_id", proveedor.IdProveedor.ToString() }
        }
    };

    var customerResponse = await _culqiService.CreateCustomerAsync(customerRequest);
    customerId = customerResponse.Id;

    // Guardar en la base de datos
    proveedor.CulqiCustomerId = customerId;
    await _proveedorRepository.UpdateAsync(proveedor);
    await _proveedorRepository.SaveAsync();
}
```

#### Paso 2: Crear Plan en Culqi (si no existe)

```csharp
var culqiPlanId = $"plan_{tarifa.IdPlanTarifa}";

var existingPlan = await _culqiService.GetPlanAsync(culqiPlanId);
if (existingPlan == null)
{
    var planRequest = new CulqiCreatePlanRequest
    {
        Id = culqiPlanId,
        Name = $"{tarifa.IdPlaneNavigation?.Nombre} - {tarifa.Nombre}",
        Amount = CulqiService.ConvertToCents(monto),
        CurrencyCode = "PEN",
        Interval = "months",
        IntervalCount = tarifa.DuracionDias >= 30 ? tarifa.DuracionDias / 30 : 1,
        Description = tarifa.Descripcion,
        Metadata = new Dictionary<string, string>
        {
            { "tarifa_id", tarifa.IdPlanTarifa.ToString() },
            { "plan_id", tarifa.IdPlane.ToString() }
        }
    };

    await _culqiService.CreatePlanAsync(planRequest);
}
```

#### Paso 3: Crear Suscripción

```csharp
var subscriptionRequest = new CulqiCreateSubscriptionRequest
{
    PlanId = culqiPlanId,
    CustomerId = customerId,
    CardId = dto.CulqiToken, // Token del frontend
    Metadata = new Dictionary<string, string>
    {
        { "plan_id", dto.IdPlane.ToString() },
        { "proveedor_id", dto.IdProveedor.ToString() },
        { "tarifa_id", dto.IdPlanTarifa.ToString() },
        { "tipo", "plan_proveedor" }
    }
};

var culqiResponse = await _culqiService.CreateSubscriptionAsync(subscriptionRequest);
```

#### Paso 4: Crear ProveedorPlan en la base de datos

```csharp
var proveedorPlan = new Entity.ProveedorPlan
{
    IdProveedor = dto.IdProveedor,
    IdPlane = dto.IdPlane,
    IdPlanTarifa = dto.IdPlanTarifa,
    FechaInicio = DateTimeOffset.UtcNow,
    FechaFin = DateTimeOffset.UtcNow.AddDays(tarifa.DuracionDias),
    FechaProximoCobro = tarifa.PermiteAutoRenovacion == true
        ? fechaFin
        : null,
    Estado = "PENDING",
    AutoRenovacion = tarifa.PermiteAutoRenovacion ?? false,
    EsActual = true,
    CulqiSubscriptionId = culqiResponse?.Id,
    CulqiCustomerId = customerId,
    GracePeriodHasta = null,
    UserNameCreate = "Sistema",
    CreateDate = DateTimeOffset.UtcNow,
    Activo = true
};

await _proveedorPlanRepository.AddAsync(proveedorPlan);
await _proveedorPlanRepository.SaveAsync();
```

### FASE 3: Webhook - Confirmación Automática

#### Configuración del Webhook en Culqi

1. Ir a [Panel Culqi](https://integ-panel.culqi.com) → Eventos → Webhooks
2. Crear nuevo webhook
3. URL: `https://tudominio.com/api/culqi/webhook`
4. Seleccionar eventos:
   - ✅ `charge.succeeded` - Pago con tarjeta exitoso
   - ✅ `charge.failed` - Pago rechazado
   - ✅ `order.status.changed` - Para Yape/Plin (QR)
   - ✅ `subscription.created` - Nueva suscripción
   - ✅ `subscription.updated` - Suscripción actualizada
   - ✅ `subscription.deleted` - Suscripción cancelada

#### Eventos de Suscripción

**Evento: `subscription.created`**

```json
{
  "id": "evt_test_123456",
  "object": "event",
  "type": "subscription.created",
  "creation_date": 1698765432000,
  "data": {
    "id": "sub_test_abc123",
    "object": "subscription",
    "plan_id": "plan_1",
    "customer_id": "cus_test_xyz789",
    "status": "active",
    "start_date": 1698765432,
    "next_billing_date": 1701357432,
    "metadata": {
      "plan_id": "2",
      "proveedor_id": "42",
      "tarifa_id": "3"
    }
  }
}
```

**Evento: `subscription.updated`**

```json
{
  "id": "evt_test_789012",
  "object": "event",
  "type": "subscription.updated",
  "creation_date": 1701357432000,
  "data": {
    "id": "sub_test_abc123",
    "object": "subscription",
    "status": "active",
    "next_billing_date": 1704035832,
    "metadata": {
      "next_billing_date": "1704035832"
    }
  }
}
```

**Evento: `subscription.deleted`**

```json
{
  "id": "evt_test_345678",
  "object": "event",
  "type": "subscription.deleted",
  "creation_date": 1704035832000,
  "data": {
    "id": "sub_test_abc123",
    "object": "subscription",
    "status": "cancelled"
  }
}
```

#### Procesamiento del Webhook

El `CulqiWebhookController` se encarga de:

1. **Validar** la firma del webhook (si Culqi la proporciona)
2. **Deserializar** el evento JSON
3. **Identificar** el tipo de evento
4. **Buscar** el `ProveedorPlan` por `CulqiSubscriptionId`
5. **Actualizar** el estado del plan:
   - `charge.succeeded` → ACTIVO, actualizar `FechaProximoCobro`
   - `charge.failed` → GRACE (5 días para pagar)
   - `subscription.deleted` → Cancelar auto-renovación
6. **Actualizar** `PagoPlan` con el estado correspondiente
7. **Enviar notificación** (email) al proveedor

---

## 📦 Componentes Implementados

### 1. Interfaz Principal

**`ICulqiService.cs`**

Define el contrato para todas las operaciones de Culqi:

```csharp
public interface ICulqiService
{
    // Pagos Únicos
    Task<CulqiChargeResponse> CreateChargeAsync(CulqiCreateChargeRequest request);

    // Suscripciones
    Task<CulqiSubscriptionResponse> CreateSubscriptionAsync(CulqiCreateSubscriptionRequest request);
    Task<bool> CancelSubscriptionAsync(string subscriptionId);
    Task<CulqiSubscriptionResponse?> GetSubscriptionAsync(string subscriptionId);

    // Planes (para suscripciones)
    Task<CulqiPlanResponse> CreatePlanAsync(CulqiCreatePlanRequest request);
    Task<CulqiPlanResponse?> GetPlanAsync(string planId);

    // Clientes (para suscripciones)
    Task<CulqiCustomerResponse> CreateCustomerAsync(CulqiCreateCustomerRequest request);
    Task<CulqiCustomerResponse?> GetCustomerAsync(string customerId);

    // Helpers
    static int ConvertToCents(decimal amount);
    static decimal ConvertToSoles(int cents);
    bool ValidateWebhookSignature(string payload, string signature);
}
```

### 2. DTOs (Data Transfer Objects)

| Archivo | Propósito |
|---------|-----------|
| `CulqiCreateChargeRequest.cs` | Request para crear cargo único |
| `CulqiChargeResponse.cs` | Respuesta de cargo único |
| `CulqiCreateSubscriptionRequest.cs` | Request para crear suscripción |
| `CulqiSubscriptionResponse.cs` | Respuesta de suscripción |
| `CulqiCreatePlanRequest.cs` | Request para crear plan en Culqi |
| `CulqiPlanResponse.cs` | Respuesta de plan creado |
| `CulqiCreateCustomerRequest.cs` | Request para crear cliente |
| `CulqiCustomerResponse.cs` | Respuesta de cliente creado |
| `CulqiWebhookEvent.cs` | Estructura del evento de webhook |
| `CulqiErrorResponse.cs` | Manejo de errores de Culqi |

### 3. Servicios

**`CulqiService.cs`** (implementa `ICulqiService`)

Métodos principales:

| Método | Endpoint Culqi | Descripción |
|--------|----------------|-------------|
| `CreateChargeAsync()` | `POST /v2/charges` | Crea un cargo único |
| `CreateSubscriptionAsync()` | `POST /v2/subscriptions` | Crea una suscripción recurrente |
| `CancelSubscriptionAsync()` | `DELETE /v2/subscriptions/{id}` | Cancela una suscripción |
| `GetSubscriptionAsync()` | `GET /v2/subscriptions/{id}` | Obtiene detalles de suscripción |
| `CreatePlanAsync()` | `POST /v2/plans` | Crea un plan para suscripciones |
| `GetPlanAsync()` | `GET /v2/plans/{id}` | Obtiene un plan |
| `CreateCustomerAsync()` | `POST /v2/customers` | Crea un cliente |
| `GetCustomerAsync()` | `GET /v2/customers/{id}` | Obtiene un cliente |
| `ConvertToCents()` | - | Convierte soles a centavos |
| `ConvertToSoles()` | - | Convierte centavos a soles |
| `ValidateWebhookSignature()` | - | Valida firma del webhook |

### 4. Controladores

**`CulqiWebhookController.cs`**

Endpoints:
- `POST /api/culqi/webhook` - Recibe notificaciones de Culqi
- `GET /api/culqi/webhook/test` - Endpoint de prueba

Eventos manejados:
- `charge.succeeded` → Actualiza `Pago` o `ProveedorPlan` a PAGADO
- `charge.failed` → Actualiza a RECHAZADO o GRACE
- `order.status.changed` → Para Yape/Plin
- `subscription.created` → Log de nueva suscripción
- `subscription.updated` → Actualiza `FechaProximoCobro`
- `subscription.deleted` → Cancela auto-renovación

**`ProveedorPlanController.cs`**

Endpoints:
- `GET /current/{idProveedor}` - Plan actual del proveedor
- `POST /checkout` - Crear nueva suscripción
- `GET /payments/{idProveedor}` - Historial de pagos
- `POST /cancel-auto-renew` - Cancelar renovación automática
- `POST /retry-payment` - Reintentar pago fallido

### 5. Commands (CQRS)

| Command | Handler | Descripción |
|---------|---------|-------------|
| `CheckoutPlanCommand` | `CheckoutPlanCommandHandler` | Crea Customer → Plan → Subscription |
| `CancelAutoRenewCommand` | `CancelAutoRenewCommandHandler` | Cancela suscripción en Culqi y BD |
| `RetryPaymentPlanCommand` | `RetryPaymentPlanCommandHandler` | Registra reintento de pago |

### 6. Queries (CQRS)

| Query | Handler | Descripción |
|-------|---------|-------------|
| `GetCurrentProveedorPlanQuery` | `GetCurrentProveedorPlanQueryHandler` | Obtiene plan actual |
| `GetPaymentsProveedorPlanQuery` | `GetPaymentsProveedorPlanQueryHandler` | Historial de pagos |

### 7. Background Services

**`PlanExpirationService.cs`**

Ejecuta cada 24 horas:
- Notifica vencimiento 1 día antes (`VENCIMIENTO_1_DIA`)
- Notifica período de gracia 5 días (`VENCIMIENTO_5_DIAS`)
- Suspende planes GRACE expirados
- Procesa renovaciones automáticas

### 8. Base de Datos

**Campos en `ProveedorPlan`:**

```sql
CulqiSubscriptionId    VARCHAR(100)  -- ID de la suscripción en Culqi
CulqiCustomerId        VARCHAR(100)  -- ID del cliente en Culqi
FechaProximoCobro      DATETIMEOFFSET -- Próxima fecha de cobro
AutoRenovacion         BIT           -- Si tiene renovación automática
GracePeriodHasta       DATETIMEOFFSET -- Límite del período de gracia
FechaCancelacion       DATETIMEOFFSET -- Fecha de cancelación
MotivoCancelacion      VARCHAR(500)  -- Motivo de cancelación
```

**Campos en `Proveedor`:**

```sql
CulqiCustomerId        VARCHAR(100)  -- ID del cliente en Culqi (cache)
```

**Campos en `PagoPlan`:**

```sql
CulqiChargeId          VARCHAR(100)  -- ID del cargo/suscripción en Culqi
CodigoOperacion        VARCHAR(50)   -- Código de referencia
FechaPago              DATETIMEOFFSET -- Fecha de pago
```

---

## ⚙️ Configuración

### 1. Obtener Credenciales de Culqi

#### Ambiente de Pruebas
1. Registrarse en https://integ-panel.culqi.com
2. Ir a **Desarrollo** → **API Keys**
3. Copiar:
   - **Clave pública**: `pk_test_XXXXXXXXXXXXXXXX`
   - **Clave secreta**: `sk_test_XXXXXXXXXXXXXXXX`

#### Ambiente de Producción
1. Entrar a https://panel.culqi.com
2. Completar verificación de cuenta
3. Ir a **Desarrollo** → **API Keys**
4. Copiar claves de producción

### 2. Configurar appsettings.json

```json
{
  "Culqi": {
    "PublicKey": "pk_test_XXXXXXXXXXXXXXXX",
    "SecretKey": "sk_test_XXXXXXXXXXXXXXXX",
    "ApiBaseUrl": "https://api.culqi.com",
    "WebhookUrl": "https://tudominio.com/api/culqi/webhook",
    "Environment": "test"
  }
}
```

⚠️ **NUNCA** commitear las claves reales a Git. Usar variables de entorno en producción.

### 3. Registro de Servicios

El servicio está registrado en `Program.cs`:

```csharp
// Culqi Service para integración de pagos
builder.Services.AddHttpClient<Reserva.Domain.Services.Culqi.CulqiService>();
builder.Services.AddScoped<Reserva.Domain.Services.Culqi.ICulqiService>(provider =>
    provider.GetRequiredService<Reserva.Domain.Services.Culqi.CulqiService>());
```

### 4. Configurar Webhook en Culqi Panel

1. Ir a **Eventos** → **Webhooks** → **+ Agregar**
2. URL: `https://tudominio.com/api/culqi/webhook`
3. Eventos a escuchar:
   - ✅ `charge.succeeded`
   - ✅ `charge.failed`
   - ✅ `order.status.changed`
   - ✅ `subscription.created`
   - ✅ `subscription.updated`
   - ✅ `subscription.deleted`
4. Guardar

---

## 🔔 Webhooks y Eventos

### Eventos Soportados

| Evento | Cuándo se dispara | Acción en el sistema |
|--------|-------------------|---------------------|
| `charge.succeeded` | Pago con tarjeta exitoso | ACTIVO plan, actualizar `PagoPlan` |
| `charge.failed` | Pago rechazado | GRACE (5 días), notificar fallo |
| `order.status.changed` | Estado de orden QR cambió | paid → ACTIVO, expired → GRACE |
| `subscription.created` | Nueva suscripción creada | Log, preparar renovación |
| `subscription.updated` | Suscripción actualizada | Actualizar `FechaProximoCobro` |
| `subscription.deleted` | Suscripción cancelada | Cancelar `AutoRenovacion` |

### Flujo de Webhook para Suscripciones

```
CULQI → POST /api/culqi/webhook
  ↓
  ├─ Validar firma (si existe)
  ├─ Deserializar JSON
  ├─ Identificar tipo de evento
  ├─ Buscar ProveedorPlan por CulqiSubscriptionId
  ├─ Si es charge.succeeded → ACTIVO, FechaProximoCobro
  ├─ Si es charge.failed → GRACE, notificar
  ├─ Si es subscription.deleted → Cancelar renovación
  └─ Retornar 200 OK
```

### Manejo de Reintentos

Culqi reintenta enviar el webhook si no recibe una respuesta 200 OK:
- **Intento 1**: Inmediato
- **Intento 2**: 5 minutos después
- **Intento 3**: 30 minutos después
- **Intento 4**: 1 hora después
- **Intento 5**: 6 horas después

⚠️ **Importante**: Siempre retornar 200 OK incluso si hay un error interno, para evitar reintentos innecesarios.

### Estados del Plan (Ciclo de Vida)

```
PENDING → (pago exitoso) → ACTIVE → (vencimiento) → GRACE → (5 días sin pago) → SUSPENDED
                                    ↓
                              (auto-renovación) → ACTIVE (nuevo ciclo)
                                    ↓
                              (cancelación) → CANCELLED
```

| Estado | Descripción |
|--------|-------------|
| `PENDING` | Pago inicial en proceso |
| `ACTIVE` | Plan activo, servicio disponible |
| `GRACE` | Pago fallido, 5 días de gracia |
| `SUSPENDED` | Período de gracia expirado |
| `CANCELLED` | Proveedor canceló la suscripción |

---

## ⚠️ Limitaciones y Consideraciones

### Limitaciones de Yape

| Limitación | Valor |
|------------|-------|
| Monto máximo | S/ 2,000 por transacción |
| Validez código | 2 minutos |
| Moneda | Solo PEN (soles) |
| Reembolsos | No soportados directamente |

### Consideraciones para Suscripciones

1. **Primer pago**: Se requiere `CulqiToken` del frontend para crear la suscripción
2. **Pagos recurrentes**: Culqi los procesa automáticamente, no requiere intervención
3. **Tarjeta guardada**: El cliente se crea en Culqi y la tarjeta queda asociada
4. **Cancelación**: El proveedor puede cancelar la renovación en cualquier momento
5. **Cambio de plan**: Se debe cancelar la suscripción actual y crear una nueva
6. **Grace Period**: 5 días para que el proveedor regularice el pago antes de suspensión

### Manejo de Errores

```csharp
try
{
    var response = await _culqiService.CreateSubscriptionAsync(request);
}
catch (CulqiException ex)
{
    // ex.Message → Mensaje técnico para logs
    // ex.UserMessage → Mensaje amigable para el usuario
    // ex.ErrorCode → Código de error de Culqi
}
```

### Testing

- Usar ambiente de integración para pruebas
- Tarjetas de prueba: https://docs.culqi.com/es/documentacion/pagos-online/testing/
- No mezclar claves de test y producción

---

## 🔧 Troubleshooting

### Problema 1: "Error: Invalid API Key"

**Causa**: Clave secreta incorrecta o de ambiente equivocado

**Solución**:
```json
{
  "Culqi": {
    "SecretKey": "sk_test_XXX",  // Para testing
    // O
    "SecretKey": "sk_live_XXX"   // Para producción
  }
}
```

### Problema 2: Webhook no se recibe

**Checklist**:
- [ ] URL es pública (no localhost)
- [ ] URL usa HTTPS
- [ ] Firewall permite requests de Culqi
- [ ] Webhook configurado en Panel Culqi
- [ ] Endpoint retorna 200 OK

**Testing local con ngrok**:
```bash
ngrok http 5000
# Usar la URL de ngrok en Culqi Panel
# Ejemplo: https://abc123.ngrok.io/api/culqi/webhook
```

### Problema 3: "Token inválido"

**Causa**: Token expirado o ya usado

**Solución**:
- Los tokens son de un solo uso
- Generar nuevo token en cada intento de pago
- Tokens expiran después de 10 minutos

### Problema 4: Suscripción queda en PENDIENTE

**Causa**: Webhook no llegó o falló el procesamiento

**Diagnóstico**:
1. Revisar logs del webhook controller
2. Verificar en Panel Culqi si el webhook se envió
3. Buscar el `CulqiSubscriptionId` en la BD

**Solución Manual**:
```sql
-- Actualizar estado manualmente después de verificar en Panel Culqi
UPDATE ProveedorPlan
SET Estado = 'ACTIVE',
    FechaProximoCobro = DATEADD(MONTH, 1, FechaInicio)
WHERE CulqiSubscriptionId = 'sub_test_abc123';
```

### Problema 5: "Amount must be at least 300 cents"

**Causa**: Culqi requiere monto mínimo de S/ 3.00

**Solución**:
```csharp
if (tarifa.Precio < 3)
{
    return BadRequest("El monto mínimo es S/ 3.00");
}
```

### Problema 6: Suscripción no se renueva automáticamente

**Causas posibles**:
1. La tarjeta del cliente fue rechazada
2. La suscripción fue cancelada en Culqi
3. El `AutoRenovacion` está desactivado

**Solución**:
1. Verificar estado en Culqi Panel
2. Revisar logs de `PlanExpirationService`
3. Notificar al proveedor para actualizar su tarjeta

---

## 📚 Recursos Adicionales

### Documentación Oficial

- **Culqi Docs**: https://docs.culqi.com/es/documentacion/
- **API Reference**: https://apidocs.culqi.com/
- **CulqiJS v4**: https://docs.culqi.com/es/documentacion/culqi-js/v4/
- **Webhooks**: https://docs.culqi.com/es/documentacion/pagos-online/webhooks/
- **Suscripciones**: https://docs.culqi.com/es/documentacion/suscripciones/

### Tarjetas de Prueba (Testing)

| Marca | Número | CVV | Fecha | Resultado |
|-------|--------|-----|-------|-----------|
| Visa | 4111 1111 1111 1111 | 123 | 09/25 | Éxito |
| Visa | 4000 0000 0000 0002 | 123 | 09/25 | Rechazo |
| Mastercard | 5111 1111 1111 1118 | 472 | 09/25 | Éxito |

### Soporte

- **Email**: soporte@culqi.com
- **Teléfono**: +51 1 644 8495
- **Horario**: Lunes a Viernes 9:00 - 18:00 (Perú)

---

**Autor**: Claude Code
**Fecha**: 2025-11-01
**Versión**: 2.0 (Con Suscripciones)
**Status**: ✅ Implementado y listo para usar
