# 📋 FLUJO DE PAGO CON CULQI

## 📌 Tabla de Contenidos
- [Introducción](#introducción)
- [Contexto de Uso](#contexto-de-uso)
- [Arquitectura General](#arquitectura-general)
- [Métodos de Pago Soportados](#métodos-de-pago-soportados)
- [Flujo Técnico Detallado](#flujo-técnico-detallado)
- [Componentes Implementados](#componentes-implementados)
- [Configuración](#configuración)
- [Uso del Servicio](#uso-del-servicio)
- [Webhooks](#webhooks)
- [Limitaciones y Consideraciones](#limitaciones-y-consideraciones)
- [Troubleshooting](#troubleshooting)

---

## 🎯 Introducción

Este documento describe la implementación completa de la integración con **Culqi**, la pasarela de pagos líder en Perú, para procesar pagos de **planes de proveedores**.

### ¿Por qué Culqi?

✅ **Acepta persona natural** - No requiere ser empresa con RUC
✅ **Comisiones competitivas** - 3.59% + S/ 0.30 por transacción
✅ **Métodos populares** - Yape, Plin, tarjetas, billeteras móviles
✅ **API bien documentada** - Integración sencilla y segura
✅ **Webhooks confiables** - Notificaciones automáticas de pagos

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

Estos planes se pagan mediante Culqi con confirmación automática.

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
│  │  5. CulqiService crea CARGO usando secret key            │   │
│  │  6. Guarda registro de pago con estado PENDIENTE         │   │
│  │  7. Retorna respuesta al frontend                        │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ Crear Cargo
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                        CULQI API                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  8. Culqi procesa el pago                                │   │
│  │  9. Usuario completa pago (Yape/Plin/Tarjeta)            │   │
│  │  10. Culqi confirma transacción                          │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ WEBHOOK (async)
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│              WEBHOOK ENDPOINT (CulqiWebhookController)          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  11. Culqi envía notificación de pago exitoso            │   │
│  │  12. Backend valida webhook                              │   │
│  │  13. Actualiza pago a PAGADO                             │   │
│  │  14. Activa el plan del proveedor                        │   │
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

### 4. **Billeteras Móviles** 📱

- **Opciones**: Otras billeteras digitales disponibles en Culqi
- **Flujo**: QR code

---

## 🔄 Flujo Técnico Detallado

### FASE 1: Frontend - Generación de Token

**Responsabilidad**: Capturar datos de pago de forma segura

```javascript
// 1. Cargar CulqiJS en tu HTML
<script src="https://checkout.culqi.com/js/v4"></script>

// 2. Configurar Culqi con tu clave pública
Culqi.publicKey = 'pk_test_XXXXXXXXXXXXXXXX';

// 3. Configurar opciones según método de pago
Culqi.settings({
  title: 'Pago de Plan Premium',
  currency: 'PEN',
  amount: 10000  // En centavos (S/ 100.00)
});

// 4. Abrir formulario de Culqi
Culqi.open();

// 5. Capturar el token generado
function culqi() {
  if (Culqi.token) {
    const token = Culqi.token.id;

    // 6. Enviar token al backend
    fetch('/api/planes/procesar-pago', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        culqi_token: token,
        plan_id: 1,
        email: 'proveedor@example.com'
      })
    })
    .then(response => response.json())
    .then(data => {
      if (data.success) {
        alert('Pago procesado exitosamente');
      }
    });
  } else if (Culqi.error) {
    console.error('Error en Culqi:', Culqi.error);
  }
}
```

---

### FASE 2: Backend - Creación del Cargo

**Responsabilidad**: Crear el cargo en Culqi usando el token

```csharp
// En tu controlador de Planes
[HttpPost("procesar-pago")]
public async Task<IActionResult> ProcesarPagoPlan([FromBody] PagarPlanDto dto)
{
    try
    {
        // 1. Obtener el plan seleccionado
        var plan = await _planRepository.GetByAsync(p => p.IdPlan == dto.IdPlan);

        // 2. Crear request para Culqi
        var culqiRequest = new CulqiCreateChargeRequest
        {
            Amount = CulqiService.ConvertToCents(plan.Precio), // S/ 100.00 → 10000
            CurrencyCode = "PEN",
            Email = dto.Email,
            SourceId = dto.CulqiToken, // Token del frontend
            Description = $"Pago de {plan.Nombre}",
            Metadata = new Dictionary<string, string>
            {
                { "plan_id", plan.IdPlan.ToString() },
                { "proveedor_id", dto.IdProveedor.ToString() }
            }
        };

        // 3. Llamar a Culqi para crear el cargo
        var culqiResponse = await _culqiService.CreateChargeAsync(culqiRequest);

        // 4. Guardar registro de pago en BD
        var pago = new Pago
        {
            IdPlan = plan.IdPlan,
            Monto = plan.Precio,
            Moneda = "PEN",
            IdEstadoPago = estadoPendiente.IdEstadoPago,
            CulqiChargeId = culqiResponse.Id,
            CulqiTokenId = dto.CulqiToken,
            CulqiReferenceCode = culqiResponse.ReferenceCode,
            UserNameCreate = User.Identity.Name
        };

        await _pagoRepository.AddAsync(pago);
        await _pagoRepository.SaveAsync();

        // 5. Retornar respuesta
        return Ok(new
        {
            success = true,
            pago_id = pago.IdPago,
            charge_id = culqiResponse.Id,
            reference_code = culqiResponse.ReferenceCode,
            message = "Pago procesado. Espera la confirmación."
        });
    }
    catch (CulqiException ex)
    {
        return BadRequest(new
        {
            success = false,
            message = ex.Message,
            user_message = ex.UserMessage
        });
    }
}
```

---

### FASE 3: Webhook - Confirmación Automática

**Responsabilidad**: Recibir notificación de Culqi y actualizar estado

#### Configuración del Webhook en Culqi

1. Ir a [Panel Culqi](https://integ-panel.culqi.com) → Eventos → Webhooks
2. Crear nuevo webhook
3. URL: `https://tudominio.com/api/culqi/webhook`
4. Seleccionar eventos:
   - ✅ `charge.succeeded` - Pago con tarjeta exitoso
   - ✅ `charge.failed` - Pago rechazado
   - ✅ `order.status.changed` - Para Yape/Plin (QR)

#### Eventos Recibidos

**Evento: `charge.succeeded`** (Tarjetas)

```json
{
  "id": "evt_test_123456",
  "object": "event",
  "type": "charge.succeeded",
  "creation_date": 1698765432000,
  "data": {
    "id": "chr_test_abc123",
    "object": "charge",
    "amount": 10000,
    "currency_code": "PEN",
    "email": "proveedor@example.com",
    "reference_code": "REF-12345",
    "metadata": {
      "plan_id": "1",
      "proveedor_id": "42"
    }
  }
}
```

**Evento: `order.status.changed`** (Yape/Plin)

```json
{
  "id": "evt_test_789012",
  "object": "event",
  "type": "order.status.changed",
  "creation_date": 1698765555000,
  "data": {
    "id": "ord_test_xyz789",
    "object": "order",
    "state": "paid",
    "amount": 5000,
    "currency_code": "PEN"
  }
}
```

#### Procesamiento del Webhook

El `CulqiWebhookController` se encarga de:

1. **Validar** la firma del webhook (si Culqi la proporciona)
2. **Deserializar** el evento JSON
3. **Identificar** el tipo de evento
4. **Buscar** el pago en la BD por `CulqiChargeId`
5. **Actualizar** el estado del pago a PAGADO
6. **Activar** el plan del proveedor
7. **Enviar notificación** (email/SMS) al proveedor

---

## 📦 Componentes Implementados

### 1. DTOs (Data Transfer Objects)

| Archivo | Propósito |
|---------|-----------|
| `CulqiCreateChargeRequest.cs` | Request para crear cargo en Culqi |
| `CulqiChargeResponse.cs` | Respuesta de Culqi al crear cargo |
| `CulqiWebhookEvent.cs` | Estructura del evento de webhook |
| `CulqiErrorResponse.cs` | Manejo de errores de Culqi |

### 2. Servicios

**`CulqiService.cs`**

Métodos principales:
- `CreateChargeAsync()` - Crea un cargo en Culqi
- `ConvertToCents()` - Convierte soles a centavos
- `ConvertToSoles()` - Convierte centavos a soles
- `ValidateWebhookSignature()` - Valida firma del webhook

### 3. Controladores

**`CulqiWebhookController.cs`**

Endpoints:
- `POST /api/culqi/webhook` - Recibe notificaciones de Culqi
- `GET /api/culqi/webhook/test` - Endpoint de prueba

### 4. Base de Datos

**Campos agregados a `Pago`:**

```sql
CulqiChargeId        NVARCHAR(100)  -- ID del cargo en Culqi
CulqiTokenId         NVARCHAR(100)  -- Token del frontend
CulqiReferenceCode   NVARCHAR(50)   -- Código de referencia
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

### 3. Ejecutar Migración SQL

```bash
# Ejecutar el script de migración
sqlcmd -S tu_servidor -d ReservaCanchas -i MIGRACION_CULQI.sql
```

O ejecutarlo manualmente en SQL Server Management Studio.

### 4. Verificar Registro de Servicio

El servicio ya está registrado en `Program.cs`:

```csharp
// Culqi Service para integración de pagos
builder.Services.AddHttpClient<Reserva.Domain.Services.Culqi.CulqiService>();
```

### 5. Configurar Webhook en Culqi Panel

1. Ir a **Eventos** → **Webhooks** → **+ Agregar**
2. URL: `https://tudominio.com/api/culqi/webhook`
3. Eventos a escuchar:
   - ✅ charge.succeeded
   - ✅ charge.failed
   - ✅ order.status.changed
4. Guardar

---

## 🚀 Uso del Servicio

### Ejemplo Completo: Procesar Pago de Plan

```csharp
public class PlanController : ControllerBase
{
    private readonly CulqiService _culqiService;
    private readonly IRepository<Pago> _pagoRepository;
    private readonly IRepository<EstadoPago> _estadoPagoRepository;
    private readonly IRepository<Plan> _planRepository;

    [HttpPost("procesar-pago-plan")]
    public async Task<IActionResult> ProcesarPagoPlan(
        [FromBody] PagarPlanDto dto)
    {
        try
        {
            // 1. Validar que el plan existe
            var plan = await _planRepository.GetByAsync(p => p.IdPlan == dto.IdPlan);
            if (plan == null)
                return NotFound("Plan no encontrado");

            // 2. Crear request para Culqi
            var culqiRequest = new CulqiCreateChargeRequest
            {
                Amount = CulqiService.ConvertToCents(plan.Precio),
                CurrencyCode = "PEN",
                Email = dto.Email,
                SourceId = dto.CulqiToken,
                Description = $"Plan {plan.Nombre} - {plan.Descripcion}",
                Metadata = new Dictionary<string, string>
                {
                    { "plan_id", plan.IdPlan.ToString() },
                    { "proveedor_id", dto.IdProveedor.ToString() },
                    { "tipo", "plan_proveedor" }
                }
            };

            // 3. Crear cargo en Culqi
            var culqiResponse = await _culqiService.CreateChargeAsync(culqiRequest);

            // 4. Obtener estado pendiente
            var estadoPendiente = await _estadoPagoRepository
                .GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_PAGO.Pendiente);

            // 5. Crear registro de pago
            var pago = new Pago
            {
                IdPlan = plan.IdPlan,
                Monto = plan.Precio,
                Moneda = "PEN",
                IdEstadoPago = estadoPendiente.IdEstadoPago,
                CulqiChargeId = culqiResponse.Id,
                CulqiTokenId = dto.CulqiToken,
                CulqiReferenceCode = culqiResponse.ReferenceCode,
                UserNameCreate = User.Identity?.Name ?? "Sistema"
            };

            await _pagoRepository.AddAsync(pago);
            await _pagoRepository.SaveAsync();

            // 6. Retornar respuesta exitosa
            return Ok(new ResponseDto<object>
            {
                Data = new
                {
                    pago_id = pago.IdPago,
                    charge_id = culqiResponse.Id,
                    reference_code = culqiResponse.ReferenceCode,
                    amount = CulqiService.ConvertToSoles(culqiResponse.Amount),
                    currency = culqiResponse.CurrencyCode,
                    status = "pending"
                },
                IsSuccess = true,
                Messages = new List<string>
                {
                    "Pago procesado exitosamente. Espera la confirmación."
                }
            });
        }
        catch (CulqiException ex)
        {
            return BadRequest(new ResponseDto<object>
            {
                IsSuccess = false,
                Messages = new List<string>
                {
                    ex.Message,
                    ex.UserMessage ?? "Error al procesar el pago"
                }
            });
        }
    }
}
```

---

## 🔔 Webhooks

### Cómo Funcionan los Webhooks

Los webhooks son notificaciones HTTP POST que Culqi envía a tu servidor cuando ocurre un evento (pago exitoso, fallido, etc.).

### Eventos Soportados

| Evento | Cuándo se dispara | Acción en el sistema |
|--------|-------------------|---------------------|
| `charge.succeeded` | Pago con tarjeta exitoso | Actualizar a PAGADO, activar plan |
| `charge.failed` | Pago rechazado | Actualizar a RECHAZADO |
| `order.status.changed` | Estado de orden QR cambió | Si state=paid → PAGADO |

### Flujo de Webhook

```
CULQI → POST /api/culqi/webhook
  ↓
  ├─ Validar firma (si existe)
  ├─ Deserializar JSON
  ├─ Identificar evento
  ├─ Buscar pago en BD
  ├─ Actualizar estado
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

### Seguridad del Webhook

Aunque Culqi no documenta públicamente el método de validación de firma, se recomienda:

1. **Validar IP de origen**: Solo aceptar requests desde IPs de Culqi
2. **Verificar payload**: Comprobar que el `charge_id` existe en tu BD
3. **Idempotencia**: Manejar webhooks duplicados correctamente

---

## ⚠️ Limitaciones y Consideraciones

### Limitaciones de Yape

| Limitación | Valor |
|------------|-------|
| Monto máximo | S/ 2,000 por transacción |
| Validez código | 2 minutos |
| Moneda | Solo PEN (soles) |
| Reembolsos | No soportados directamente |

### Consideraciones Importantes

1. **Manejo de Errores**
   - Siempre capturar `CulqiException`
   - Mostrar `UserMessage` al usuario final
   - Logear `MerchantMessage` para debugging

2. **Idempotencia**
   - Los webhooks pueden llegar duplicados
   - Verificar estado actual antes de actualizar
   - Usar `CulqiChargeId` como clave única

3. **Timeout**
   - Los pagos tienen un timeout de 15 minutos
   - Después de ese tiempo, el pago expira
   - Manejar estado "expirado"

4. **Reembolsos**
   - Los reembolsos deben hacerse desde el Panel Culqi
   - No hay API pública para reembolsos automáticos
   - Implementar flujo manual de reembolsos

5. **Testing**
   - Usar ambiente de integración para pruebas
   - Tarjetas de prueba: https://docs.culqi.com/es/documentacion/pagos-online/testing/
   - No mezclar claves de test y producción

---

## 🔧 Troubleshooting

### Problema 1: "Error: Invalid API Key"

**Causa**: Clave secreta incorrecta o de ambiente equivocado

**Solución**:
```json
// Verificar que la clave coincida con el ambiente
{
  "Culqi": {
    "SecretKey": "sk_test_XXX",  // Para testing
    // O
    "SecretKey": "sk_live_XXX"   // Para producción
  }
}
```

### Problema 2: Webhook no se recibe

**Causa**: URL no accesible desde Internet o no configurada en Culqi

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

### Problema 4: Pago queda en PENDIENTE

**Causa**: Webhook no llegó o falló el procesamiento

**Diagnóstico**:
1. Revisar logs del webhook controller
2. Verificar en Panel Culqi si el webhook se envió
3. Buscar el `CulqiChargeId` en la BD

**Solución Manual**:
```sql
-- Actualizar estado manualmente después de verificar en Panel Culqi
UPDATE Pago
SET IdEstadoPago = (SELECT IdEstadoPago FROM EstadoPago WHERE Codigo = '01'), -- Pagado
    CulqiReferenceCode = 'REF-12345'
WHERE CulqiChargeId = 'chr_test_abc123';
```

### Problema 5: "Amount must be at least 300 cents"

**Causa**: Culqi requiere monto mínimo de S/ 3.00

**Solución**:
```csharp
// Validar monto antes de enviar a Culqi
if (plan.Precio < 3)
{
    return BadRequest("El monto mínimo es S/ 3.00");
}
```

---

## 📚 Recursos Adicionales

### Documentación Oficial

- **Culqi Docs**: https://docs.culqi.com/es/documentacion/
- **API Reference**: https://apidocs.culqi.com/
- **CulqiJS v4**: https://docs.culqi.com/es/documentacion/culqi-js/v4/
- **Webhooks**: https://docs.culqi.com/es/documentacion/pagos-online/webhooks/

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

## ✅ Checklist de Implementación

Antes de ir a producción, verificar:

- [ ] Credenciales de producción configuradas
- [ ] Webhook configurado en Panel Culqi producción
- [ ] Migración SQL ejecutada en BD producción
- [ ] Variables de entorno configuradas correctamente
- [ ] HTTPS habilitado en el dominio
- [ ] Logs de errores configurados
- [ ] Testing completo con tarjetas de prueba
- [ ] Manejo de errores implementado
- [ ] Notificaciones por email configuradas
- [ ] Documentación de endpoints actualizada

---

**Autor**: Claude Code
**Fecha**: 2025-11-01
**Versión**: 1.0
**Status**: ✅ Implementado y listo para usar
