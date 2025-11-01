# 🚀 Guía Rápida - Integración Culqi

## ✅ ¿Qué se ha implementado?

Se ha creado una **integración completa con Culqi** para procesar pagos de **planes de proveedores** (NO para reservas). La implementación incluye:

### 📦 Archivos Creados

#### 1. DTOs y Modelos (Reserva.Domain/Services/Culqi/)
- `CulqiCreateChargeRequest.cs` - Request para crear cargos
- `CulqiChargeResponse.cs` - Respuesta de Culqi
- `CulqiWebhookEvent.cs` - Eventos de webhook
- `CulqiErrorResponse.cs` - Manejo de errores

#### 2. Servicio Principal
- `CulqiService.cs` - Servicio para comunicación con API de Culqi
  - `CreateChargeAsync()` - Crear cargo
  - `ConvertToCents()` / `ConvertToSoles()` - Conversiones
  - `ValidateWebhookSignature()` - Validación de webhooks

#### 3. Controlador de Webhooks
- `CulqiWebhookController.cs` - Recibe notificaciones de Culqi
  - `POST /api/culqi/webhook` - Endpoint principal
  - `GET /api/culqi/webhook/test` - Endpoint de prueba

#### 4. Base de Datos
- `MIGRACION_CULQI.sql` - Script de migración
  - Agrega campos: `CulqiChargeId`, `CulqiTokenId`, `CulqiReferenceCode`
  - Crea índice para búsquedas rápidas

#### 5. Configuración
- `appsettings.json` - Actualizado con sección Culqi
- `Program.cs` - Registra CulqiService

#### 6. Documentación
- `FLUJO_PAGO_CULQI.md` - Documentación completa

---

## 🎯 Pasos para Usar

### Paso 1: Ejecutar Migración SQL

```bash
# Opción 1: Desde SQL Server Management Studio
# Abrir MIGRACION_CULQI.sql y ejecutar

# Opción 2: Desde línea de comandos
sqlcmd -S tu_servidor -d ReservaCanchas -i MIGRACION_CULQI.sql
```

### Paso 2: Obtener Credenciales de Culqi

**Para Testing:**
1. Ir a https://integ-panel.culqi.com
2. Registrarse / Iniciar sesión
3. Ir a **Desarrollo** → **API Keys**
4. Copiar:
   - Clave pública: `pk_test_XXXXXXXXXXXXXXXX`
   - Clave secreta: `sk_test_XXXXXXXXXXXXXXXX`

### Paso 3: Configurar appsettings.json

```json
{
  "Culqi": {
    "PublicKey": "pk_test_XXXXXXXXXXXXXXXX",  // ← Pegar aquí
    "SecretKey": "sk_test_XXXXXXXXXXXXXXXX",   // ← Pegar aquí
    "ApiBaseUrl": "https://api.culqi.com",
    "WebhookUrl": "https://tudominio.com/api/culqi/webhook",
    "Environment": "test"
  }
}
```

⚠️ **Importante**: No commitear las claves reales a Git

### Paso 4: Configurar Webhook en Culqi

1. Ir a Panel Culqi → **Eventos** → **Webhooks**
2. Crear nuevo webhook
3. URL: `https://tudominio.com/api/culqi/webhook`
4. Seleccionar eventos:
   - ✅ `charge.succeeded`
   - ✅ `charge.failed`
   - ✅ `order.status.changed`

**Para testing local con ngrok:**
```bash
ngrok http 5000
# Usar la URL de ngrok: https://abc123.ngrok.io/api/culqi/webhook
```

---

## 💻 Ejemplo de Uso en tu Código

### En tu Controlador de Planes

```csharp
public class PlanController : ControllerBase
{
    private readonly CulqiService _culqiService;
    private readonly IRepository<Pago> _pagoRepository;
    private readonly IRepository<Plan> _planRepository;

    [HttpPost("procesar-pago")]
    public async Task<IActionResult> ProcesarPago([FromBody] PagarPlanDto dto)
    {
        try
        {
            // 1. Obtener el plan
            var plan = await _planRepository.GetByAsync(p => p.IdPlan == dto.IdPlan);

            // 2. Crear request para Culqi
            var request = new CulqiCreateChargeRequest
            {
                Amount = CulqiService.ConvertToCents(plan.Precio),
                CurrencyCode = "PEN",
                Email = dto.Email,
                SourceId = dto.CulqiToken, // Token del frontend
                Description = $"Plan {plan.Nombre}",
                Metadata = new Dictionary<string, string>
                {
                    { "plan_id", plan.IdPlan.ToString() }
                }
            };

            // 3. Crear cargo en Culqi
            var response = await _culqiService.CreateChargeAsync(request);

            // 4. Guardar pago en BD
            var pago = new Pago
            {
                IdPlan = plan.IdPlan,
                Monto = plan.Precio,
                Moneda = "PEN",
                CulqiChargeId = response.Id,
                CulqiTokenId = dto.CulqiToken,
                CulqiReferenceCode = response.ReferenceCode,
                IdEstadoPago = estadoPendiente.IdEstadoPago
            };

            await _pagoRepository.AddAsync(pago);
            await _pagoRepository.SaveAsync();

            return Ok(new { success = true, pago_id = pago.IdPago });
        }
        catch (CulqiException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

### En tu Frontend

```javascript
// 1. Cargar CulqiJS
<script src="https://checkout.culqi.com/js/v4"></script>

// 2. Configurar
Culqi.publicKey = 'pk_test_XXXXXXXXXXXXXXXX'; // Tu clave pública

// 3. Abrir checkout
Culqi.settings({
  title: 'Pago de Plan',
  currency: 'PEN',
  amount: 10000  // S/ 100.00 en centavos
});
Culqi.open();

// 4. Capturar token
function culqi() {
  if (Culqi.token) {
    // Enviar token al backend
    fetch('/api/planes/procesar-pago', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        culqi_token: Culqi.token.id,
        plan_id: 1,
        email: 'user@example.com'
      })
    });
  }
}
```

---

## 🔍 Verificar que Funciona

### 1. Probar Webhook

```bash
curl http://localhost:5000/api/culqi/webhook/test
```

Debe retornar:
```json
{
  "message": "Webhook de Culqi funcionando correctamente",
  "timestamp": "2025-11-01T..."
}
```

### 2. Probar Creación de Cargo

Usa las tarjetas de prueba de Culqi:
- **Visa exitosa**: 4111 1111 1111 1111, CVV: 123, Fecha: 09/25
- **Visa rechazada**: 4000 0000 0000 0002, CVV: 123, Fecha: 09/25

### 3. Verificar Logs

```csharp
// Los logs se guardan automáticamente
// Revisar en consola o archivo de logs
```

---

## 📋 Métodos de Pago Soportados

| Método | Límites | Notas |
|--------|---------|-------|
| **Yape** 🟡 | Máx. S/ 2,000 | Código válido 2 min |
| **Plin** 🔵 | Variable | QR code |
| **Tarjetas** 💳 | Sin límite | Visa, Mastercard, etc. |
| **Billeteras** 📱 | Variable | Otras billeteras |

---

## ⚠️ Importante Recordar

1. **Separación de Responsabilidades**
   - ✅ Culqi: Para **planes de proveedores**
   - ❌ NO usar Culqi para **reservas** (solo efectivo)

2. **Seguridad**
   - Nunca exponer la `SecretKey` en el frontend
   - Solo usar `PublicKey` en el cliente
   - `SecretKey` solo en backend

3. **Webhook es Crítico**
   - Sin webhook, los pagos quedan en PENDIENTE
   - El webhook actualiza automáticamente a PAGADO
   - Configurarlo correctamente en Panel Culqi

4. **Montos en Centavos**
   - Culqi usa centavos: S/ 100.00 = 10000
   - Usar `CulqiService.ConvertToCents()` siempre

5. **Testing**
   - Usar ambiente de integración (`pk_test_`, `sk_test_`)
   - No mezclar claves de test y producción

---

## 🆘 ¿Problemas?

Revisar la documentación completa en:
- `FLUJO_PAGO_CULQI.md` - Documentación detallada
- Sección **Troubleshooting** con soluciones comunes

---

## 📚 Próximos Pasos

1. **Crear Entidad Plan** (si no existe)
   - Tabla para almacenar planes de proveedores
   - Campos: Nombre, Descripción, Precio, Duración, etc.

2. **Implementar Activación de Plan**
   - Después del webhook de pago exitoso
   - Activar el plan del proveedor automáticamente

3. **Notificaciones**
   - Email al proveedor cuando se confirma el pago
   - SMS de confirmación (opcional)

4. **Dashboard de Proveedores**
   - Ver estado del plan actual
   - Historial de pagos
   - Renovación de plan

---

**¿Listo para empezar?** 🎉

1. ✅ Ejecuta la migración SQL
2. ✅ Configura las credenciales
3. ✅ Configura el webhook
4. ✅ Empieza a procesar pagos

**Documentación completa**: Ver `FLUJO_PAGO_CULQI.md`
