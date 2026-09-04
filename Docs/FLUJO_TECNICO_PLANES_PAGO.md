# Flujo Técnico - Sistema de Planes y Pagos (v5.0)

> **Versión**: 5.0 | **Fecha**: 2026-09-04
> 
> **Cambios v4.0**: Detección de método de pago (Yape vs Tarjeta), plan Culqi dinámico, prorrateo con pago único
> **Cambios v4.1**: `esPagoUnico` se determina por `tarifa.Codigo` (no por `PaymentType`). Soporte Yape en planes de suscripción.
> **Cambios v4.2**: PagoPlan se crea en webhook (no en handlers). Cada charge genera PagoPlan histórico. Búsqueda unificada por metadata proveedor_id. FechaFin usa DateTimeHelper.GetNextBillingDate.
> **Cambios v5.0**: Cancelación diferida para cambios de plan. Campo `CulqiSubscriptionIdAnterior`. Manejo de reintentos Culqi. `ChangePlanCommandHandler` deprecated - se usa `CheckoutPlanCommandHandler`.

## Resumen de Flujos

| # | Escenario | Componente | Handler | Endpoint | Método Pago |
|---|-----------|------------|---------|----------|-------------|
| 1 | Plan único (UNIQUE/BLACKFRIDAY) - Tarjeta | `planes-catalogo` | `CheckoutPlanCommandHandler` | `/checkout` | Card → Charge |
| 1B | Plan único (UNIQUE/BLACKFRIDAY) - Yape | `planes-catalogo` | `CheckoutPlanCommandHandler` | `/checkout` | Order → Charge |
| 1C | Plan suscripción (MONTHLY/YEARLY) - Tarjeta | `planes-catalogo` | `CheckoutPlanCommandHandler` | `/checkout` | Card → Subscription |
| 1D | Plan suscripción (MONTHLY/YEARLY) - Yape | `planes-catalogo` | `CheckoutPlanCommandHandler` | `/checkout` | Order → Charge (sin renovación) |
| 2 | GRACE → Pagar en ver-plan | `ver-plan` | `RetryPaymentPlanCommandHandler` | `/retry-payment` | Card/Order |
| 3 | GRACE → Catálogo → Plan actual | `planes-catalogo` | `RetryPaymentPlanCommandHandler` | `/retry-payment` | Card/Order |
| 4 | **ACTIVE → Cambio de plan (via Checkout)** | `planes-catalogo` | `CheckoutPlanCommandHandler` | `/checkout` | Card/Order (con cancelación diferida) |
| 5 | ACTIVE → Cancelar renovación | `ver-plan` | `CancelAutoRenewCommandHandler` | `/cancel-auto-renew` | N/A |
| 6 | **Webhooks Culqi** | - | `CulqiWebhookController` | `/webhook/culqi` | - |

> **NOTA**: `ChangePlanCommandHandler` está deprecated. Los cambios de plan ahora se procesan a través de `CheckoutPlanCommandHandler` con cancelación diferida de la suscripción anterior.

---

## Concepto Clave: Tipo de Plan vs Método de Pago

> **IMPORTANTE**: El tipo de plan (único vs suscripción) se determina por el **código de tarifa**, NO por el método de pago del usuario.

| Código Tarifa | Tipo Plan | Culqi Plan | Subscription | Comportamiento |
|---------------|-----------|------------|--------------|----------------|
| `UNIQUE` | Pago único | No creado | No creada | Siempre Charge, sin renovación |
| `BLACKFRIDAY` | Pago único | No creado | No creada | Siempre Charge, sin renovación |
| `MONTHLY` | Suscripción | Creado | Creada | Customer+Card+Subscription |
| `YEARLY` | Suscripción | Creado | Creada | Customer+Card+Subscription |

**Flujo según combinación:**

| Plan | PaymentType | Resultado |
|------|-------------|-----------|
| UNIQUE | `card` | Charge (pago único con tarjeta) |
| UNIQUE | `order` | Charge (pago único con Yape) |
| MONTHLY | `card` | Customer+Card+Subscription (renovación automática) |
| MONTHLY | `order` | Customer+Charge (sin renovación, usuario debe agregar tarjeta) |

---

## Campo Clave: CulqiSubscriptionIdAnterior (Cancelación Diferida)

Cuando un usuario con plan activo cambia a otro plan, **NO cancelamos inmediatamente la suscripción anterior**. En su lugar:

1. **CheckoutPlanCommandHandler** guarda la referencia de la suscripción anterior en `CulqiSubscriptionIdAnterior`
2. **Webhook charge.creation.succeeded** cancela la suscripción anterior DESPUÉS de confirmar que el nuevo pago fue exitoso
3. **Webhook charge.creation.failed** cancela el nuevo plan y limpia la referencia (el plan anterior se mantiene ACTIVE)

```
CAMPO: ProveedorPlan.CulqiSubscriptionIdAnterior
  → Contiene el ID de la suscripción Culqi anterior (solo en cambios de plan)
  → Se setea en: CheckoutPlanCommandHandler (al cambiar de plan)
  → Se lee en: CulqiWebhookController.HandlePlanPaymentSucceeded()
  → Se limpia en: CulqiWebhookController.HandlePlanPaymentFailed() o HandlePlanPaymentSucceeded()
  → Efecto: Permite cancelación diferida sin perder servicio del plan anterior
```

**Flujo de cambio de plan:**
```
ANTES (v4.x - Problemático):
  Checkout → CancelSubscription(anterior) → Crear nueva → Si falla → Usuario pierde servicio

AHORA (v5.0 - Corregido):
  Checkout → Guardar CulqiSubscriptionIdAnterior → Crear nueva
    ├─ Si éxito → charge.succeeded → CancelSubscription(anterior) → Plan anterior CANCELLED
    └─ Si falla → charge.failed → Nuevo plan CANCELLED → Plan anterior ACTIVE (sin cambio)
```

---

## Manejo de Reintentos Culqi

Culqi reintenta automáticamente los cobros fallidos de suscripciones. Nuestra lógica maneja esto:

| Estado | Primer Fallo | Reintentos Culqi | Acción |
|--------|--------------|------------------|--------|
| **PENDING** | CANCELLED | **NO reintenta** | Cancelamos suscripción en Culqi para evitar reintentos |
| **ACTIVE (renovación)** | GRACE | **Sí reintenta** | Culqi reintenta; si paga → vuelve a ACTIVE |
| **ACTIVE (cambio plan)** | CANCELLED | **NO reintenta** | Cancelamos suscripción para evitar reintentos |
| **GRACE** | Mantener GRACE | **Sí reintenta** | Culqi reintenta; si paga → vuelve a ACTIVE |

**¿Por qué cancelamos suscripción en PENDING?**
- El usuario aún no tiene servicio activo
- Si el pago falla, puede intentar pagar de nuevo desde el catálogo
- Cancelar la suscripción evita que Culqi siga reintentando cobros fallidos

**¿Por qué NO cancelamos en GRACE/ACTIVE (renovación)?**
- El usuario ya tiene servicio activo
- Culqi debe seguir reintentando para recuperar el pago
- Si el pago se recupera, el plan vuelve a ACTIVE automáticamente

---

## Métodos de Pago - Detección en Culqi Checkout v4

### Cómo detecta Culqi el método de pago

```
┌─────────────────────────────────────────────────────────────────────────┐
│  CULQUI CHECKOUT v4 - Respuesta del modal                               │
└─────────────────────────────────────────────────────────────────────────┘

  function culqi() {
    if (Culqi.token) {
      // TARJETA → token.id = "tkn_live_xxx"
      return { type: 'card', id: token.id };
    } else if (Culqi.order) {
      // YAPE/PagoEfectivo/Billeteras → order.id = "ord_xxx"
      return { type: 'order', id: order.id };
    }
  }
```

### Flujo según método de pago

```
┌─────────────────────────────────────────────────────────────────────────┐
│  PAYMENT TYPE: 'card' (TARJETA)                                         │
├─────────────────────────────────────────────────────────────────────────┤
│  Token ID: "tkn_live_xxx"                                               │
│                                                                         │
│  Backend:                                                               │
│  ├── Crear Customer en Culqi (si no existe)                            │
│  ├── Crear Card con token                                              │
│  ├── Crear Subscription con Plan + Customer + Card                     │
│  ├── ProveedorPlan.Estado = PENDING                                    │
│  ├── ProveedorPlan.AutoRenovacion = true                               │
│  └── Culqi cobra automáticamente en FechaProximoCobro                  │
│                                                                         │
│  Webhook: charge.succeeded → Estado = ACTIVE                            │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│  PAYMENT TYPE: 'order' (YAPE/PAGO ÚNICO)                               │
├─────────────────────────────────────────────────────────────────────────┤
│  Token ID: "ype_live_xxx" (Yape) o "ord_xxx" (otro)                    │
│                                                                         │
│  Backend:                                                               │
│  ├── NO crear Customer                                                 │
│  ├── NO crear Card                                                     │
│  ├── NO crear Subscription                                             │
│  ├── Crear Charge directo con source_id = token                        │
│  ├── ProveedorPlan.Estado = ACTIVE (directo)                           │
│  ├── ProveedorPlan.AutoRenovacion = false                              │
│  └── Sin cobros automáticos futuros                                    │
│                                                                         │
│  Webhook: charge.succeeded → Confirma pago, plan ya está ACTIVE        │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## FLUJO 1: Primera Vez - Registro de Plan

### Descripción
Proveedor nuevo selecciona un plan del catálogo y se suscribe por primera vez.

**Importante**: El tipo de plan (único vs suscripción) se determina por el **código de tarifa**, no por el método de pago. Un plan UNIQUE siempre será pago único, sin importar si el usuario paga con tarjeta o Yape.

### Componentes Involucrados
- **Frontend**: `planes-catalogo.component.ts`, `culqi.service.ts`
- **Backend**: `CheckoutPlanCommandHandler.cs`
- **Servicio Culqi**: `CulqiService.cs`

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────────────┐
│  USUARIO                                                               │
│  1. Navega a /planes/catalogo                                           │
│  2. Selecciona un plan                                                  │
│  3. Click "Seleccionar Plan"                                            │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  FRONTEND - planes-catalogo.component.ts                                │
│                                                                         │
│  onSeleccionarPlan(plan)                                                │
│  ├── getTarifaForPlan(plan) → obtiene tarifa                           │
│  ├── if (isPlanActualEnGrace) → retryPayment (Flujo 3)                 │
│  ├── if (isPlanActual && ACTIVE) → onCambiarPlan (Flujo 4)             │
│  └── else → onComprarPlan(plan, tarifa)                                │
│                                                                         │
│  onComprarPlan(plan, tarifa)                                            │
│  ├── culqiService.loadScript()                                         │
│  ├── culqiService.openCheckout({                                       │
│  │     title, currency, amount, description, email                     │
│  │   })                                                                │
│  │   → Retorna: { type: 'card' | 'order', id: string }                │
│  │                                                                     │
│  └── proveedorPlanService.checkout({                                   │
│        idProveedor, idPlane, idPlanTarifa,                             │
│        culqiToken: result.id,                                          │
│        paymentType: result.type,  // 'card' o 'order'                  │
│        email                                                             │
│      })                                                                │
│      → POST /api/ProveedorPlan/checkout                                │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - CheckoutPlanCommandHandler.cs                                │
│                                                                         │
│  HandleCommand(request)                                                 │
│  │                                                                      │
│  ├── 1. VALIDACIONES                                                    │
│  │   ├── tarifa existe?                                                 │
│  │   └── proveedor existe?                                              │
│  │                                                                      │
│  ├── 2. CALCULAR MONTO                                                  │
│  │   monto = tarifa.Precio - (descuento si aplica)                     │
│  │                                                                      │
│  ├── 3. DETERMINAR TIPO DE PLAN (según CÓDIGO de tarifa)               │
│  │   ├── esPagoUnico = tarifa.Codigo is "UNIQUE" or "BLACKFRIDAY"      │
│  │   │   → SIEMPRE crear Charge, nunca Subscription                    │
│  │   └── else (MONTHLY/YEARLY) → Plan de suscripción                   │
│  │                                                                      │
│  ├── 4. DETERMINAR MÉTODO DE PAGO DEL USUARIO                          │
│  │   ├── esPagoConTarjeta = dto.PaymentType == "card"                  │
│  │   └── (PaymentType solo afecta cómo se paga, no el tipo de plan)   │
│  │                                                                      │
│  ├── 5. SI ES PLAN DE PAGO ÚNICO (UNIQUE/BLACKFRIDAY)                  │
│  │   ├── Crear Charge con token (puede ser tarjeta o Yape)             │
│  │   ├── NO crear Customer, NO Card, NO Subscription                   │
│  │   ├── Estado = ACTIVE directo                                       │
│  │   └── AutoRenovacion = false                                        │
│  │                                                                      │
│  ├── 6. SI ES PLAN DE SUSCRIPCIÓN (MONTHLY/YEARLY)                     │
│  │   │                                                                  │
│  │   ├── 6a. Si esPagoConTarjeta (card):                               │
│  │   │   ├── Crear Customer en Culqi (si no existe)                    │
│  │   │   ├── Crear Card con token                                      │
│  │   │   ├── Crear Plan Culqi (si debe)                                │
│  │   │   ├── Crear Subscription                                         │
│  │   │   ├── Estado = PENDING (esperando webhook)                       │
│  │   │   └── AutoRenovacion = true                                     │
│  │   │                                                                  │
│  │   └── 6b. Si esPagoYape (order):                                    │
│  │       ├── Crear Customer en Culqi (si no existe)                    │
│  │       ├── Crear Charge con token Yape                               │
│  │       ├── NO crear Card, NO crear Subscription                      │
│  │       ├── Estado = ACTIVE (pago directo)                             │
│  │       ├── AutoRenovacion = false                                    │
│  │       └── NOTA: Usuario debe agregar tarjeta para renovación       │
│  │                                                                      │
│  ├── 7. CREAR PROVEEDOR PLAN                                            │
│  │   ├── FechaInicio = ahora                                            │
│  │   ├── FechaFin = DateTimeHelper.GetNextBillingDate(ahora, ahora.Day, tarifa.DuracionDias)                        │
│  │   ├── FechaProximoCobro = solo si esSuscripcionConTarjeta           │
│  │   └── CulqiSubscriptionId = solo si esSuscripcionConTarjeta         │
│  │                                                                      │
│  └── 8. CREAR PAGO PLAN ← ELIMINADO (webhook lo crea)                    │
       (PagoPlan se crea en CulqiWebhookController al recibir             │
       charge.creation.succeeded con charge ID real: ch_001, ch_002...)   │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  CULQI - Procesamiento                                                  │
│                                                                         │
│  Si es TARJETA + suscripción:                                           │
│  ├── Cobra al cliente con la tarjeta                                    │
│  ├── Envía webhook charge.succeeded → Estado = ACTIVE                   │
│  └── Envía webhook charge.failed → Estado = GRACE                       │
│                                                                         │
│  Si es YAPE (cualquier plan):                                           │
│  ├── Charge ya fue creado (pago directo)                                │
│  ├── Webhook charge.succeeded → Confirma pago                           │
│  └── Plan ya está ACTIVE, no hay cobros futuros                         │
└─────────────────────────────────────────────────────────────────────────┘
│  │   └── Retornar: "Suscripción iniciada. Esperando confirmación..."       │
│  │   (tarjeta) o "Pago registrado correctamente." (pago único)           │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  CULQI - Procesamiento                                                  │
│                                                                         │
│  Si es TARJETA:                                                         │
│  ├── Cobra al cliente con la tarjeta                                    │
│  ├── Envía webhook charge.succeeded → Estado = ACTIVE                   │
│  └── Envía webhook charge.failed → Estado = GRACE                       │
│                                                                         │
│  Si es YAPE (pago único):                                               │
│  ├── Charge ya fue creado (pago directo)                                │
│  ├── Webhook charge.succeeded → Confirma pago                           │
│  └── Plan ya está ACTIVE, no hay cobros futuros                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Código Clave

**Frontend** (`culqi.service.ts`):
```typescript
openCheckout(options: CulqiCheckoutOptions): Promise<CulqiCheckoutResult> {
  return new Promise((resolve) => {
    (window as any).Culqi.open(options);
    (window as any).culqi = () => {
      if ((window as any).Culqi.token) {
        resolve({ type: 'card', id: (window as any).Culqi.token.id });
      } else if ((window as any).Culqi.order) {
        resolve({ type: 'order', id: (window as any).Culqi.order.id });
      }
    };
  });
}
```

**Backend** (`CheckoutPlanCommandHandler.cs`):
```csharp
// Determinar método de pago
var esPagoUnico = request.PaymentType == "order";
var codigoMetodoPago = esPagoUnico
    ? Constants.METODO_PAGO.Yape
    : Constants.METODO_PAGO.Tarjeta;

if (esPagoUnico) {
    // Pago único: solo crear charge
    var chargeRequest = new CulqiCreateChargeRequest {
        Amount = (int)(monto * 100),
        CurrencyCode = "PEN",
        Email = proveedor.Correo,
        SourceId = request.CulqiToken,
        Metadata = new Dictionary<string, string> {
            { "tipo", "pago_unico" },
            { "proveedor_id", request.IdProveedor.ToString() }
        }
    };
    var chargeResponse = await _culqiService.CreateChargeAsync(chargeRequest);
    proveedorPlan.Estado = Constants.ESTADO_PROVEEDOR_ACTIVO;
    proveedorPlan.AutoRenovacion = false;
} else {
    // Tarjeta: crear customer, card, subscription
    // ... (flujo completo de suscripción)
}
```

---

## FLUJO 2: Estado GRACE → Pagar en Ver Plan

### Descripción
Proveedor con plan vencido (estado GRACE) paga directamente desde la página de ver plan.

### Componentes Involucrados
- **Frontend**: `ver-plan.component.ts`
- **Backend**: `RetryPaymentPlanCommandHandler.cs`
- **Servicio Culqi**: `CulqiService.cs`

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────────────┐
│  USUARIO                                                               │
│  1. Navega a /admin/planes                                             │
│  2. Ve estado GRACE con botón "Pagar plan - S/ 99.00"                  │
│  3. Click "Pagar plan"                                                 │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  FRONTEND - ver-plan.component.ts                                       │
│                                                                         │
│  onPagarPlan()                                                          │
│  ├── culqiService.loadScript()                                         │
│  ├── culqiService.openCheckout({                                       │
│  │     title: `Pagar plan - ${plan.nombre}`,                           │
│  │     currency: 'PEN',                                                │
│  │     amount: getMontoPagar() * 100,                                  │
│  │     description: `Pago plan ${plan.nombre}`,                        │
│  │     email                                                           │
│  │   })                                                                │
│  │   → Retorna: token                                                  │
│  │                                                                     │
│  └── onReintentarPago(token)                                           │
│      │                                                                 │
│      ├── proveedorPlanService.retryPayment({                           │
│      │     idProveedorPlan,                                            │
│      │     culqiToken: token,                                          │
│      │     email                                                       │
│      │   })                                                            │
│      │   → POST /api/ProveedorPlan/retry-payment                       │
│      │                                                                 │
│      └── Si éxito → startPolling() (verificar estado cada 5s)          │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - RetryPaymentPlanCommandHandler.cs                            │
│                                                                         │
│  HandleCommand(request)                                                 │
│  │                                                                      │
│  ├── 1. VALIDACIONES                                                    │
│  │   ├── ProveedorPlan existe?                                         │
│  │   ├── Estado es GRACE o PAST_DUE?                                   │
│  │   └── Proveedor existe?                                             │
│  │                                                                      │
│  ├── 2. CALCULAR MONTO                                                  │
│  │   monto = tarifa.Precio - (descuento si aplica)                     │
│  │                                                                      │
│  ├── 3. ACTUALIZAR TARJEDA (si se envió token)                         │
│  │   ├── if (CulqiToken != null && CulqiCustomerId != null)            │
│  │   │   ├── GetCardAsync(customerId) → existingCard                   │
│  │   │   ├── CreateCardAsync(customerId, token) → newCard              │
│  │   │   ├── DeleteCardAsync(existingCard.Id)                          │
│  │   │   │                                                             │
│  │   │   ├── if (CulqiSubscriptionId != null)                          │
│  │   │   │   └── UpdateSubscriptionAsync(subscriptionId, {             │
│  │   │   │         CardId: newCard.Id                                  │
│  │   │   │       })                                                    │
│  │   │   │                                                             │
│  │   │   └── Registrar log: "Tarjeta anterior eliminada"               │
│  │                                                                      │
│  ├── 4. REGISTRAR PAGO ← ELIMINADO (webhook lo crea)                      │
│  │   (PagoPlan se crea en CulqiWebhookController al recibir             │
│  │    charge.creation.succeeded con charge ID real)                    │                               │
│  └── Retornar: "Pago registrado. Culqi procesará automáticamente."     │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  CULQI - Procesamiento                                                  │
│                                                                         │
│  1. Usa la tarjeta actualizada de la suscripción                        │
│  2. Cobra al cliente                                                    │
│  3. Envía webhook de resultado                                          │
│  4. Estado cambia: GRACE → ACTIVE (si éxito)                           │
│                    GRACE → GRACE otra vez (si falla)                    │
└─────────────────────────────────────────────────────────────────────────┘
```

### Código Clave

**Frontend** (`ver-plan.component.ts`):
```typescript
async onPagarPlan(): Promise<void> {
  await this.culqiService.loadScript();
  
  const token = await this.culqiService.openCheckout({
    title: `Pagar plan - ${this.planActual.plan.nombre}`,
    currency: 'PEN',
    amount: this.getMontoPagar() * 100,
    description: `Pago plan ${this.planActual.plan.nombre}`,
    email: email
  });

  this.onReintentarPago(token);
}

onReintentarPago(culqiToken?: string): void {
  this.proveedorPlanService.retryPayment({
    idProveedorPlan: this.planActual.idProveedorPlan,
    culqiToken: culqiToken || null,
    email: culqiToken ? email : null
  }).subscribe({
    next: (response) => {
      if (response.isValid) {
        this.openSuccessAlert('Pago registrado. Esperando confirmación...');
        this.startPolling();
      }
    }
  });
}
```

**Backend** (`RetryPaymentPlanCommandHandler.cs`):
```csharp
// Actualizar tarjeta
var existingCard = await _culqiService.GetCardAsync(proveedor.CulqiCustomerId);
var newCard = await _culqiService.CreateCardAsync(proveedor.CulqiCustomerId, dto.CulqiToken);

if (existingCard != null)
{
    await _culqiService.DeleteCardAsync(existingCard.Id);
}

// Actualizar suscripción
var updateRequest = new CulqiUpdateSubscriptionRequest
{
    CardId = newCard.Id
};
await _culqiService.UpdateSubscriptionAsync(proveedorPlan.CulqiSubscriptionId, updateRequest);

// Registrar pago
var pagoPlan = new PagoPlan
{
    Monto = monto,
    CulqiChargeId = proveedorPlan.CulqiSubscriptionId
};
await _pagoPlanRepository.AddAsync(pagoPlan);
```

---

## FLUJO 3: Estado GRACE → Catálogo → Plan Actual

### Descripción
Proveedor con plan vencido (estado GRACE) va al catálogo y selecciona su mismo plan para pagarlo.

### Componentes Involucrados
- **Frontend**: `planes-catalogo.component.ts`
- **Backend**: `RetryPaymentPlanCommandHandler.cs`
- **Servicio Culqi**: `CulqiService.cs`

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────────────┐
│  USUARIO                                                               │
│  1. Navega a /planes/catalogo                                           │
│  2. Ve su plan actual con botón "💳 Pagar plan actual"                  │
│  3. Click "Pagar plan actual"                                          │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  FRONTEND - planes-catalogo.component.ts                                │
│                                                                         │
│  onSeleccionarPlan(plan)                                                │
│  ├── if (isPlanActualEnGrace(plan))                                    │
│  │   └── onPagarPlanActual()  ← Flujo 3                               │
│  │                                                                      │
│  onPagarPlanActual()                                                    │
│  ├── culqiService.loadScript()                                         │
│  ├── planActual = planes.find(p => p.idPlane === idPlanActual)         │
│  ├── tarifa = getTarifaForPlan(planActual)                             │
│  │                                                                      │
│  ├── culqiService.openCheckout({                                       │
│  │     title: `Pagar plan - ${planActual.nombre}`,                     │
│  │     currency: tarifa.moneda,                                        │
│  │     amount: tarifa.precio * 100,                                    │
│  │     description: `Pago plan ${planActual.nombre}`,                  │
│  │     email                                                           │
│  │   })                                                                │
│  │   → Retorna: token                                                  │
│  │                                                                     │
│  └── proveedorPlanService.retryPayment({                               │
│        idProveedorPlan,                                                │
│        culqiToken: token,                                              │
│        email                                                           │
│      })                                                                │
│      → POST /api/ProveedorPlan/retry-payment                           │
│      → Si éxito → startPolling()                                       │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - RetryPaymentPlanCommandHandler.cs                            │
│  (MISMO CÓDIGO QUE FLUJO 2)                                            │
│                                                                         │
│  HandleCommand(request)                                                 │
│  │                                                                      │
│  ├── 1. VALIDACIONES                                                    │
│  ├── 2. CALCULAR MONTO                                                  │
│  ├── 3. ACTUALIZAR TARJEDA (si se envió token)                         │
│  └── 4. REGISTRAR PAGO                                                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Diferencias con Flujo 2

| Aspecto | Flujo 2 | Flujo 3 |
|---------|---------|---------|
| **Punto de entrada** | ver-plan.component | planes-catalogo.component |
| **Método** | `onPagarPlan()` | `onPagarPlanActual()` |
| **Backend** | `RetryPaymentPlanCommandHandler` | `RetryPaymentPlanCommandHandler` |
| **Lógica** | Idéntica | Idéntica |

### Lógica de Botones en Catálogo

```typescript
// planes-catalogo.component.ts

isPlanActual(plan: ListPlaneDto): boolean {
  return plan.idPlane === this.idPlanActual && this.estado === 'ACTIVE';
}

isPlanActualEnGrace(plan: ListPlaneDto): boolean {
  return plan.idPlane === this.idPlanActual && this.estado === 'GRACE';
}

canSeleccionar(plan: ListPlaneDto): boolean {
  if (!this.idPlanActual || this.estado === 'CANCELLED' || this.estado === 'SUSPENDED') {
    return true;
  }
  if (this.estado === 'GRACE' && plan.idPlane === this.idPlanActual) {
    return true;  // Permitir pagar plan actual en GRACE
  }
  return plan.idPlane !== this.idPlanActual;
}

getBotonTexto(plan: ListPlaneDto): string {
  if (this.isPlanActual(plan)) return 'Plan Actual';
  if (this.isPlanActualEnGrace(plan)) return 'Pagar plan actual';
  // ...
}
```

---

## FLUJO 4: Estado ACTIVE → Cambio de Plan (Upgrade/Downgrade) - v5.0

### Descripción
Proveedor con plan activo cambia a un plan diferente (superior o inferior). **Ahora se procesa a través de `CheckoutPlanCommandHandler`** con cancelación diferida de la suscripción anterior.

**Ventajas de la nueva implementación:**
- El plan anterior se mantiene ACTIVO hasta que el nuevo pago sea confirmado
- Si el nuevo pago falla, el usuario conserva su servicio actual
- La suscripción anterior solo se cancela después de confirmar el nuevo pago

**Importante**: El tipo del **nuevo plan** se determina por su código de tarifa:
- Si el nuevo plan es UNIQUE/BLACKFRIDAY → pago único (Charge)
- Si el nuevo plan es MONTHLY/YEARLY → suscripción (Customer+Card+Subscription)

**Concepto clave:** `FechaFin` = fecha de expiración de acceso (nuestra lógica). `FechaProximoCobro` = billing anchor de Culqi (webhook la actualiza). **Culqi NUNCA toca `FechaFin`.**

### Componentes Involucrados
- **Frontend**: `planes-catalogo.component.ts`, `culqi.service.ts`
- **Backend**: `CalculateProrationQueryHandler.cs` + **`CheckoutPlanCommandHandler.cs`** (NO ChangePlanCommandHandler)
- **Webhook**: `CulqiWebhookController.cs`
- **Servicio Culqi**: `CulqiService.cs`

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────────────┐
│  USUARIO                                                               │
│  1. Navega a /planes/catalogo                                           │
│  2. Ve su plan actual con badge "Plan Actual"                          │
│  3. Selecciona un plan diferente (upgrade o downgrade)                  │
│  4. Click "Cambiar a este plan"                                         │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  FRONTEND - planes-catalogo.component.ts                                │
│                                                                         │
│  onSeleccionarPlan(plan)                                                │
│  ├── if (isPlanActual(plan) && estado === 'ACTIVE')                     │
│  │   └── onCambiarPlan(plan)  ← Flujo 4                                │
│  │                                                                      │
│  onCambiarPlan(plan: ListPlaneDto)                                      │
│  │                                                                      │
│  ├── 1. CALCULAR PRORRATEO (Query)                                      │
│  │   ├── proveedorPlanService.calculateProration({                      │
│  │   │     idProveedorPlanActual,                                       │
│  │   │     idPlanNuevo: plan.idPlane,                                   │
│  │   │     idPlanTarifaNueva                                            │
│  │   │   })                                                             │
│  │   │   → GET /api/ProveedorPlan/calculate-proration                   │
│  │   │   → Retorna: CalculateProrationResponseDto                       │
│  │   │     {                                                            │
│  │   │       montoProrrateo,      // Monto a cobrar (upgrade)           │
│  │   │       diasRestantes,       // Días restantes del ciclo actual     │
│  │   │       saldoAFavor,         // Crédito para downgrade             │
│  │   │       esUpgrade,           // true = upgrade, false = downgrade  │
│  │   │       fechaFinCiclo        // Fecha fin del ciclo actual         │
│  │   │     }                                                            │
│  │   │                                                                  │
│  │   └── showConfirmationModal(prorationData)                           │
│  │                                                                      │
│  ├── 2. MODAL DE CONFIRMACIÓN                                           │
│  │   └── Muestra:                                                       │
│  │       - Plan actual vs nuevo plan                                    │
│  │       - Días restantes del ciclo                                     │
│  │       - Monto a cobrar (upgrade) o saldo a favor (downgrade)         │
│  │       - Botón "Confirmar cambio"                                     │
│  │                                                                      │
│  └── 3. EJECUTAR CAMBIO (después de confirmar)                         │
│      │                                                                  │
│      ├── if (esUpgrade)                                                 │
│      │   ├── culqiService.loadScript()                                  │
│      │   ├── culqiService.openCheckout({                                │
│      │   │     title: `Cambio de plan - ${planNuevo.nombre}`,          │
│      │   │     currency: 'PEN',                                         │
│      │   │     amount: prorationData.montoProrrateo * 100,             │
│      │   │     description: `Prorrateo días restantes`,                 │
│      │   │     email                                                    │
│      │   │   })                                                         │
│      │   │   → Retorna: { type: 'card' | 'order', id: string }        │
│      │   │                                                              │
│      │   └── executeCheckoutPlan(plan, result)                          │
│      │                                                                  │
│      └── else (downgrade)                                               │
│          └── executeCheckoutPlan(plan, null)  // Sin cobro              │
│                                                                         │
│  executeCheckoutPlan(plan, culqiResult?)                                │
│  ├── proveedorPlanService.checkout({                                    │
│  │     idProveedor,                                                     │
│  │     idPlanNuevo: plan.idPlane,                                       │
│  │     idPlanTarifaNueva,                                               │
│  │     culqiToken: culqiResult?.id,                                     │
│  │     paymentType: culqiResult?.type ?? 'card',                        │
│  │     email                                                            │
│  │   })                                                                 │
│  │   → POST /api/ProveedorPlan/checkout                                │
│  │   → Si éxito → mostrar mensaje éxito + refresh                       │
│  └──────────────────────────────────────────────────────────────────────┘
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - CheckoutPlanCommandHandler.cs (v5.0)                         │
│                                                                         │
│  HandleCommand(request)                                                 │
│  │                                                                      │
│  ├── 1. VALIDACIONES                                                    │
│  │   ├── Tarifa existe?                                                 │
│  │   └── Proveedor existe?                                              │
│  │                                                                      │
│  ├── 2. CALCULAR MONTO                                                  │
│  │   monto = tarifa.Precio - (descuento si aplica)                     │
│  │                                                                      │
│  ├── 3. DETERMINAR TIPO DE PLAN                                         │
│  │   ├── esPagoUnico = tarifa.Codigo is "UNIQUE" or "BLACKFRIDAY"      │
│  │   └── else (MONTHLY/YEARLY) → Plan de suscripción                   │
│  │                                                                      │
│  ├── 4. BUSCAR SUSCRIPCIÓN ANTERIOR (si existe plan activo)            │
│  │   ├── Buscar ProveedorPlan activo del proveedor                     │
│  │   ├── Si tiene CulqiSubscriptionId → guardar referencia             │
│  │   └── Marcar plan anterior con MotivoCancelacion =                   │
│  │       "PENDIENTE_CANCELACION_CAMBIO_PLAN"                           │
│  │                                                                      │
│  ├── 5. PROCESAR PAGO                                                   │
│  │   ├── Si esPagoUnico: Crear Charge                                  │
│  │   ├── Si esPagoConTarjeta: Customer + Card + Subscription           │
│  │   └── Si esYape: Customer + Charge (sin subscription)               │
│  │                                                                      │
│  ├── 6. CREAR NUEVO PROVEEDOR PLAN                                     │
│  │   ├── Estado = PENDING (suscripción) o ACTIVE (pago único)          │
│  │   ├── CulqiSubscriptionId = nueva suscripción (si aplica)           │
│  │   ├── CulqiSubscriptionIdAnterior = referencia anterior             │
│  │   └── NO cancelar suscripción anterior aún (cancelación diferida)   │
│  │                                                                      │
│  └── Retornar resultado                                                 │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  WEBHOOK - CulqiWebhookController.cs                                    │
│                                                                         │
│  ═══ SI CHARGE.SUCCEEDED (pago exitoso): ═══                            │
│  │                                                                      │
│  ├── 1. Activar nuevo plan (PENDING → ACTIVE)                          │
│  ├── 2. Crear PagoPlan con charge ID real                              │
│  ├── 3. Si tiene CulqiSubscriptionIdAnterior:                          │
│  │   ├── CancelSubscriptionAsync(suscripción anterior)                 │
│  │   ├── Buscar plan anterior por CulqiSubscriptionId                  │
│  │   ├── Marcar plan anterior como CANCELLED                           │
│  │   └── Limpiar CulqiSubscriptionIdAnterior                          │
│  └── 4. Notificar éxito al proveedor                                   │
│                                                                         │
│  ═══ SI CHARGE.FAILED (pago fallido): ═══                              │
│  │                                                                      │
│  ├── Buscar ProveedorPlan más reciente (OrderByDescending)            │
│  │                                                                      │
│  ├── Si Estado == PENDING:                                              │
│  │   ├── CANCELLED (plan nuevo falló)                                  │
│  │   ├── Cancelar suscripción en Culqi (evitar reintentos)            │
│  │   ├── Si tiene CulqiSubscriptionIdAnterior → limpiar referencia    │
│  │   └── Plan anterior se mantiene ACTIVE ✓                            │
│  │                                                                      │
│  ├── Si Estado == ACTIVE + tiene CulqiSubscriptionIdAnterior:          │
│  │   ├── CANCELLED (cambio de plan fallido)                            │
│  │   ├── Cancelar suscripción en Culqi                                │
│  │   └── Plan anterior se mantiene ACTIVE ✓                            │
│  │                                                                      │
│  └── Si Estado == ACTIVE sin referencia (renovación):                  │
│      ├── GRACE (período de gracia)                                     │
│      ├── NO cancelar suscripción (Culqi reintenta)                    │
│      └── Culqi reintenta cobros automáticos                           │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Lógica de Cancelación Diferida

```
TIMELINE - Cambio de Plan Exitoso:
──────────────────────────────────────────────────────
T0: Checkout
    ├── Plan A (ACTIVE) → MotivoCancelacion = "PENDIENTE_CANCELACION_CAMBIO_PLAN"
    ├── Plan B (PENDING) → CulqiSubscriptionIdAnterior = subscription_A
    └── Culqi cobra al cliente

T1: Webhook charge.succeeded
    ├── Plan B → ACTIVE
    ├── CancelSubscriptionAsync(subscription_A)
    ├── Plan A → CANCELLED
    └── CulqiSubscriptionIdAnterior = null

RESULTADO: Sin interrupción de servicio ✓

TIMELINE - Cambio de Plan Fallido:
──────────────────────────────────────────────────────
T0: Checkout
    ├── Plan A (ACTIVE) → MotivoCancelacion = "PENDIENTE_CANCELACION_CAMBIO_PLAN"
    ├── Plan B (PENDING) → CulqiSubscriptionIdAnterior = subscription_A
    └── Culqi intenta cobrar

T1: Webhook charge.failed
    ├── Plan B → CANCELLED
    ├── CancelSubscriptionAsync(subscription_B) ← Evita reintentos
    ├── CulqiSubscriptionIdAnterior = null
    └── Plan A → SE MANTIENE ACTIVE ✓

RESULTADO: Usuario conserva su plan actual ✓
```

### Código Clave

**Frontend** (`planes-catalogo.component.ts`):
```typescript
async executeCheckoutPlan(plan: ListPlaneDto, proration: CalculateProrationResponseDto): Promise<void> {
  let token: string | null = null;

  // Si es upgrade, cobrar prorrateo
  if (proration.esUpgrade && proration.montoProrrateo > 0) {
    await this.culqiService.loadScript();
    token = await this.culqiService.openCheckout({
      title: `Cambio de plan - ${plan.nombre}`,
      currency: 'PEN',
      amount: proration.montoProrrateo * 100,
      description: `Prorrateo ${proration.diasRestantes} días restantes`,
      email: this.email
    });
  }

  // Ejecutar checkout (mismo endpoint que primera vez)
  this.proveedorPlanService.checkout({
    idProveedor: this.idProveedor,
    idPlane: plan.idPlane,
    idPlanTarifa: plan.idPlanTarifa,
    culqiToken: token,
    email: this.email
  }).subscribe({
    next: (response) => {
      if (response.isValid) {
        this.openSuccessAlert('Cambio de plan procesado. Esperando confirmación...');
        this.startPolling();
      }
    }
  });
}
```

**Backend** (`CheckoutPlanCommandHandler.cs` - Sección de cambio de plan):
```csharp
// Buscar suscripción anterior del proveedor
var oldProveedorPlan = await _proveedorPlanRepository.GetByAsync(
    x => x.IdProveedor == dto.IdProveedor
        && x.Activo
        && x.Estado != Constants.ESTADO_PROV_PLAN.CANCELLED
        && !string.IsNullOrEmpty(x.CulqiSubscriptionId),
    x => x.IdPlaneNavigation
);

string? oldSubscriptionId = null;
if (oldProveedorPlan != null)
{
    // Guardar referencia para cancelación diferida
    oldSubscriptionId = oldProveedorPlan.CulqiSubscriptionId;
    
    // Marcar plan anterior (se cancelará después del webhook)
    oldProveedorPlan.MotivoCancelacion = "PENDIENTE_CANCELACION_CAMBIO_PLAN";
    await _proveedorPlanRepository.UpdateAsync(oldProveedorPlan);
}

// Crear nuevo ProveedorPlan
var proveedorPlan = new Entity.ProveedorPlan
{
    // ... otros campos ...
    CulqiSubscriptionId = culqiSubscriptionId,
    CulqiSubscriptionIdAnterior = oldSubscriptionId  // ← CLAVE
    // NO cancelamos suscripción anterior aquí
};
```
│  │     paymentType: culqiResult?.type ?? 'card',                        │
│  │     email                                                            │
│  │   })                                                                 │
│  │   → POST /api/ProveedorPlan/change-plan                              │
│  │   → Si éxito → mostrar mensaje éxito + refresh                       │
│  └──────────────────────────────────────────────────────────────────────┘
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - CalculateProrationQueryHandler.cs                             │
│                                                                         │
│  HandleQuery(request)                                                   │
│  │                                                                      │
│  ├── 1. OBTENER PROVEEDOR PLAN ACTUAL                                  │
│  │   ├── ProveedorPlan = repository.GetById(request.IdProveedorPlan)   │
│  │   └── Validar que existe y está activo                               │
│  │                                                                      │
│  ├── 2. OBTENER TARIFAS                                                │
│  │   ├── TarifaActual = PlanTarifaRepository.Get(                       │
│  │   │       proveedorPlan.IdPlanTarifa)                                │
│  │   └── TarifaNueva = PlanTarifaRepository.Get(                        │
│  │           request.IdPlanTarifaNueva)                                 │
│  │                                                                      │
│  ├── 3. CALCULAR DÍAS                                                   │
│  │   ├── fechaFinCiclo = proveedorPlan.FechaFin                        │
│  │   ├── hoy = DateTime.Now                                             │
│  │   ├── diasTotales = tarifaActual.DuracionDias                       │
│  │   └── diasRestantes = (fechaFinCiclo - hoy).Days                    │
│  │                                                                      │
│  ├── 4. CALCULAR PRORRATEO                                              │
│  │   ├── creditoPlanActual = precioActual / diasTotales * diasRestantes│
│  │   ├── cargoPlanNuevo = precioNuevo / duracionNueva * diasRestantes  │
│  │   ├── saldoAFavorAnterior = proveedorPlan.SaldoFavor                 │
│  │   └── montoProrrateo = cargoPlanNuevo - creditoPlanActual           │
│  │                        - saldoAFavorAnterior                         │
│  │                                                                      │
│  │   NOTA:                                                               │
│  │   - Si montoProrrateo > 0 → Es UPGRADE (cobrar diferencia)         │
│  │   - Si montoProrrateo < 0 → Es DOWNGRADE (guardar saldo a favor)   │
│  │   - Si montoProrrateo = 0 → Sin cambio de monto                     │
│  │                                                                      │
│  └── Retornar CalculateProrationResponseDto                             │
│      {                                                                  │
│        MontoProrrateo = Math.Abs(montoProrrateo),                      │
│        DiasRestantes,                                                   │
│        SaldoAFavor = montoProrrateo < 0 ? Math.Abs(montoProrrateo) : 0 │
│        EsUpgrade = montoProrrateo >= 0,                                 │
│        FechaFinCiclo                                                    │
│      }                                                                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - ChangePlanCommandHandler.cs                                  │
│                                                                         │
│  HandleCommand(request)                                                 │
│  │                                                                      │
│  ├── 1. VALIDACIONES                                                    │
│  │   ├── ProveedorPlan existe?                                         │
│  │   ├── Estado es ACTIVE?                                              │
│  │   ├── EsActual == true?                                              │
│  │   └── Nueva tarifa existe?                                           │
│  │                                                                      │
│  ├── 2. DETERMINAR TIPO DE NUEVO PLAN (según CÓDIGO de tarifa)         │
│  │   ├── esPagoUnico = nuevaTarifa.Codigo is "UNIQUE"/"BLACKFRIDAY"    │
│  │   └── else → Plan de suscripción                                    │
│  │                                                                      │
│  │   NOTA: Si el plan actual era suscripción, siempre cancelar sub     │
│  │                                                                      │
│  ├── 3. CALCULAR PRORRATEO (misma lógica que Query)                    │
│  │   └── Recalcular montoProrrateo y esUpgrade                         │
│  │                                                                      │
│  ├── 4. DETERMINAR MÉTODO DE PAGO DEL USUARIO                          │
│  │   ├── esPagoConTarjeta = request.PaymentType == "card"              │
│  │   └── (PaymentType solo afecta cómo se paga, no el tipo de plan)   │
│  │                                                                      │
│  ├── 5. CANCELAR SUSCRIPCIÓN ANTERIOR (si existía)                     │
│  │   ├── Si proveedorPlan.CulqiSubscriptionId != null                  │
│  │   │   └── CancelSubscriptionAsync(CulqiSubscriptionId)              │
│  │   └── Marcar plan anterior como CANCELLED                           │
│  │                                                                      │
│  ├── 6. COBRAR PRORRATEO SI UPGRADE                                    │
│  │   ├── Si esPagoUnico o !esPagoConTarjeta:                           │
│  │   │   └── Crear Charge con token (Yape o tarjeta)                   │
│  │   └── Si esPagoConTarjeta:                                          │
│  │       ├── Actualizar Card                                           │
│  │       └── Crear Charge con CustomerId                               │
│  │                                                                      │
│  ├── 7. SI NUEVO PLAN ES SUSCRIPCIÓN CON TARJETA                       │
│  │   ├── Crear Plan Culqi (si debe)                                    │
│  │   └── Crear Subscription                                             │
│  │                                                                      │
│  ├── 8. CREAR NUEVO PROVEEDOR PLAN                                    │
│  │   ├── FechaInicio = ahora                                           │
│  │   ├── FechaFin = DateTimeHelper.GetNextBillingDate(ahora, ahora.Day, duracionNueva)                              │
│  │   ├── FechaProximoCobro = solo si esSuscripcionConTarjeta           │
│  │   ├── Estado = ACTIVE (pago directo) o PENDING (suscripción)        │
│  │   ├── CulqiSubscriptionId = solo si esSuscripcionConTarjeta         │
│  │   ├── AutoRenovacion = solo si esSuscripcionConTarjeta              │
│  │   ├── EsActual = true                                               │
│  │   └── SaldoAFavor = nuevoSaldoAFavor (solo downgrade)               │
│  │   └── PagoPlan ← NO se crea aquí (webhook lo crea con charge real)  │
│  │                                                                      │
│  └── Retornar ChangePlanResponseDto                                    │
│      {                                                                  │
│        IdProveedorPlan, Estado, NuevaFechaFin,                         │
│        SaldoAFavor, EsUpgrade, MontoProrrateado                        │
│      }                                                                  │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  CULQI - Procesamiento                                                  │
│                                                                         │
│  Si es TARJETA + suscripción:                                           │
│  ├── 1. Se recibe webhook subscription.created                         │
│  │   → Se setea FechaProximoCobro                                      │
│  └── 2. charge.succeeded → Estado = ACTIVE                             │
│                                                                         │
│  Si es YAPE o plan único:                                               │
│  ├── Charge ya fue creado (pago directo)                                │
│  └── Plan ya está ACTIVE, no hay cobros futuros                         │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  CULQI - Webhook: subscription.created                                  │
│                                                                         │
│  1. Se recibe webhook con next_billing_date                            │
│  2. Se setea proveedorPlan.FechaProximoCobro                           │
│  3. Estado sigue PENDING hasta charge.succeeded                        │
│                                                                         │
│  LUEGO: charge.succeeded                                                │
│  1. Estado cambia: PENDING → ACTIVE                                    │
│  2. Se marca PagoPlan como Pagado                                      │
│  3. Se envía notificación de activación                                │
└─────────────────────────────────────────────────────────────────────────┘
```

### Fórmula de Prorrateo

```
EJEMPLO - UPGRADE (Mensual S/100 → Trimestral S/250):
- Plan actual: S/ 100/mes (30 días)
- Plan nuevo: S/ 250/trimestre (90 días)
- Días restantes: 10 días
- Saldo a favor anterior: S/ 0

Cálculo:
1. Crédito plan actual = (100 / 30) × 10 = S/ 33.33
2. Cargo plan nuevo = (250 / 90) × 10 = S/ 27.78
3. Monto prorrateo = 27.78 - 33.33 - 0 = -S/ 5.55
4. Como es negativo → DOWNGRADE → Saldo a favor = S/ 5.55

EJEMPLO - UPGRADE (Mensual S/100 → Anual S/900):
- Plan actual: S/ 100/mes (30 días)
- Plan nuevo: S/ 900/anual (365 días)
- Días restantes: 10 días

Cálculo:
1. Crédito plan actual = (100 / 30) × 10 = S/ 33.33
2. Cargo plan nuevo = (900 / 365) × 10 = S/ 24.66
3. Monto prorrateo = 24.66 - 33.33 = -S/ 8.67
4. DOWNGRADE → Saldo a favor = S/ 8.67

EJEMPLO - UPGRADE (Mensual S/50 → Trimestral S/250):
- Plan actual: S/ 50/mes (30 días)
- Plan nuevo: S/ 250/trimestre (90 días)
- Días restantes: 10 días

Cálculo:
1. Crédito plan actual = (50 / 30) × 10 = S/ 16.67
2. Cargo plan nuevo = (250 / 90) × 10 = S/ 27.78
3. Monto prorrateo = 27.78 - 16.67 = S/ 11.11
4. UPGRADE → Cobrar S/ 11.11
```

### Código Clave

**Frontend** (`planes-catalogo.component.ts`):
```typescript
async onCambiarPlan(plan: ListPlaneDto): Promise<void> {
  // 1. Calcular prorrateo
  const proration = await firstValueFrom(
    this.proveedorPlanService.calculateProration({
      idProveedorPlanActual: this.idProveedorPlanActual,
      idPlanNuevo: plan.idPlane,
      idPlanTarifaNueva: plan.idPlanTarifa
    })
  );

  // 2. Mostrar modal de confirmación
  const confirmed = await this.confirmarCambioPlan(plan, proration);
  
  if (!confirmed) return;

  // 3. Ejecutar checkout (mismo endpoint que primera vez)
  await this.executeCheckoutPlan(plan, proration);
}

async executeCheckoutPlan(plan: ListPlaneDto, proration: CalculateProrationResponseDto): Promise<void> {
  let token: string | null = null;

  // Si es upgrade, cobrar prorrateo
  if (proration.esUpgrade && proration.montoProrrateo > 0) {
    await this.culqiService.loadScript();
    token = await this.culqiService.openCheckout({
      title: `Cambio de plan - ${plan.nombre}`,
      currency: 'PEN',
      amount: proration.montoProrrateo * 100,
      description: `Prorrateo ${proration.diasRestantes} días restantes`,
      email: this.email
    });
  }

  // Ejecutar checkout (mismo endpoint que primera vez)
  this.proveedorPlanService.checkout({
    idProveedor: this.idProveedor,
    idPlane: plan.idPlane,
    idPlanTarifa: plan.idPlanTarifa,
    culqiToken: token,
    paymentType: token ? 'card' : 'order',
    email: this.email
  }).subscribe({
    next: (response) => {
      if (response.isValid) {
        this.openSuccessAlert('Cambio de plan procesado. Esperando confirmación...');
        this.startPolling();
      }
    }
  });
}
```

**Backend** (`CheckoutPlanCommandHandler.cs` - Sección de cambio de plan):
```csharp
// Buscar suscripción anterior del proveedor
var oldProveedorPlan = await _proveedorPlanRepository.GetByAsync(
    x => x.IdProveedor == dto.IdProveedor
        && x.Activo
        && x.Estado != Constants.ESTADO_PROV_PLAN.CANCELLED
        && !string.IsNullOrEmpty(x.CulqiSubscriptionId),
    x => x.IdPlaneNavigation
);

string? oldSubscriptionId = null;
if (oldProveedorPlan != null)
{
    // Guardar referencia para cancelación diferida
    oldSubscriptionId = oldProveedorPlan.CulqiSubscriptionId;
    
    // Marcar plan anterior (se cancelará después del webhook)
    oldProveedorPlan.MotivoCancelacion = "PENDIENTE_CANCELACION_CAMBIO_PLAN";
    await _proveedorPlanRepository.UpdateAsync(oldProveedorPlan);
}

// Crear nuevo ProveedorPlan
var proveedorPlan = new Entity.ProveedorPlan
{
    IdProveedor = dto.IdProveedor,
    IdPlane = dto.IdPlane,
    IdPlanTarifa = dto.IdPlanTarifa,
    FechaInicio = fechaInicio,
    FechaFin = fechaFin,
    Estado = Constants.ESTADO_PROV_PLAN.PENDING,
    CulqiSubscriptionId = culqiSubscriptionId,
    CulqiCustomerId = customerId,
    CulqiSubscriptionIdAnterior = oldSubscriptionId  // ← CLAVE
    // NO cancelamos suscripción anterior aquí
};
```

### Modelo de Facturación - Separación de Conceptos

```
IMPORTANTE: FechaFin y FechaProximoCobro son CONCEPTOS DISTINTOS.

FechaFin (FechaExpiracionPlan):
  → Cuándo EXPIRA el ACCESO al plan
  → La calculamos NOSOTROS: FechaInicio + DuracionNuevaTarifa
  → Culqi NUNCA la toca
  → Se usa para: notificaciones de vencimiento, GRACE, SUSPENDED

FechaProximoCobro (Billing Anchor):
  → Cuándo Culqi intenta COBRAR
  → La setea Culqi via webhook (next_billing_date)
  → Se actualiza después de cada cobro exitoso
  → Se usa para: facturación recurrente

FLUJO CORRECTO:
  15 FEB: Cambio Mensual → Trimestral
    ├── FechaInicio = 15 FEB (hoy)
    ├── FechaFin = 15 FEB + 90 días = 15 MAY  ← ACCESO
    └── FechaProximoCobro = null (webhook la setea)

  Webhook subscription.created (llega después):
    └── FechaProximoCobro = next_billing_date Culqi

  15 MAY: FechaFin llega
    ├── 14 MAY: Notificación "plan expira mañana"
    ├── 15 MAY: Plan pasa a GRACE
    └── 20 MAY: Plan pasa a SUSPENDED

  Webhook subscription.updated (si cobro exitoso):
    └── FechaProximoCobro = next_billing_date Culqi
    └── FechaFin NO cambia (intacta)
```

### Diferencias con Otros Flujos

| Aspecto | Flujo 1 | Flujo 2/3 | Flujo 4 | Flujo 5 |
|---------|---------|-----------|---------|---------|
| **Estado inicial** | Sin plan | GRACE | ACTIVE | ACTIVE |
| **Operación** | Crear suscripción | Reintentar pago | Cancelar + Nueva suscripción | Solo cancelar |
| **Culqi** | Crear | retryPayment | DELETE + CREATE | DELETE solamente |
| **Cobro** | Suscripción | Monto completo | Charge + Nueva suscripción | Ninguno |
| **FechaFin** | Nueva | Se mantiene | Nueva | Se mantiene |
| **Estado final** | PENDING | ACTIVE | PENDING | ACTIVE (hasta FechaFin) |
| **Después de FechaFin** | N/A | N/A | N/A | CANCELLED directo |
| **CancelAtPeriodEnd** | false | false | false | true |

---

## FLUJO 5: Estado ACTIVE → Cancelar Renovación

### Descripción
Proveedor decide cancelar la renovación automática de su plan. La suscripción Culqi se elimina permanentemente. El plan sigue activo hasta `FechaFin`, luego pasa directamente a CANCELLED (sin GRACE, sin notificaciones).

### Componentes Involucrados
- **Frontend**: `ver-plan.component.ts`
- **Backend**: `CancelAutoRenewCommandHandler.cs`, `PlanExpirationService.cs`
- **Servicio Culqi**: `CancelSubscriptionAsync()` → DELETE /v2/subscriptions/{id}

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────────────┐
│  USUARIO                                                               │
│  1. Navega a /planes/ver-plan/{id}                                     │
│  2. Ve badge "Renovación automática activa"                            │
│  3. Click "Cancelar Renovación"                                        │
│  4. Confirma en modal de confirmación                                   │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  FRONTEND - ver-plan.component.ts                                       │
│                                                                         │
│  onCancelarRenovacion()                                                 │
│  ├── canCancelRenovacion():                                             │
│  │   ├── estado === 'ACTIVE'                                            │
│  │   ├── autoRenovacion === true                                        │
│  │   └── cancelAtPeriodEnd === false  // Ya cancelada?                  │
│  │                                                                      │
│  ├── Modal de confirmación:                                            │
│  │   ├── "¿Cancelar renovación automática?"                            │
│  │   ├── "Tu plan seguirá activo hasta {fechaFin}"                      │
│  │   └── "No se generarán más cobros automáticos"                      │
│  │                                                                      │
│  └── proveedorPlanService.cancelarRenovacion(idProveedorPlan)          │
│      → POST /api/ProveedorPlan/cancel-auto-renew/{id}                  │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  BACKEND - CancelAutoRenewCommandHandler.cs                             │
│                                                                         │
│  HandleCommand(request)                                                 │
│  │                                                                      │
│  ├── 1. VALIDACIONES                                                    │
│  │   ├── ProveedorPlan existe?                                          │
│  │   ├── Pertenecen al mismo UserIdNegocio?                            │
│  │   ├── Estado == ACTIVE?                                              │
│  │   ├── AutoRenovacion == true?                                        │
│  │   └── CancelAtPeriodEnd == false?                                    │
│  │                                                                      │
│  ├── 2. CANCELAR EN CULQI                                               │
│  │   ├── CulqiSubscriptionId no es null/negativo?                       │
│  │   ├── CancelSubscriptionAsync(CulqiSubscriptionId)                   │
│  │   │   → DELETE /v2/subscriptions/{id}                                │
│  │   │   → Culqi retorna suscripción con status: cancelled              │
│  │   └── Log: "Suscripción Culqi cancelada"                            │
│  │                                                                      │
│  ├── 3. ACTUALIZAR PROVEEDOR PLAN EN BD                                 │
│  │   ├── proveedorPlan.AutoRenovacion = false                           │
│  │   ├── proveedorPlan.CancelAtPeriodEnd = true    ← NUEVO CAMPO        │
│  │   └── SaveChangesAsync()                                             │
│  │                                                                      │
│  └── Retornar CancelAutoRenewResponseDto                                │
│      {                                                                  │
│        Success = true,                                                  │
│        Mensaje = "Renovación cancelada exitosamente",                   │
│        FechaFin = proveedorPlan.FechaFin                                │
│      }                                                                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  PLAN EXPIRATION SERVICE (Background Service - Timer)                   │
│                                                                         │
│  ProcesarCancelacionesAlFinPeriodo() ← NUEVO MÉTODO                    │
│  │                                                                      │
│  ├── Query: Todos los planes con:                                       │
│  │   ├── CancelAtPeriodEnd == true                                      │
│  │   ├── FechaFin < now                                                │
│  │   └── Estado != CANCELLED                                            │
│  │                                                                      │
│  ├── Para cada plan:                                                    │
│  │   ├── proveedorPlan.Estado = CANCELLED                               │
│  │   ├── proveedorPlan.EsActual = false                                 │
│  │   ├── proveedorPlan.FechaCancelacion = now                           │
│  │   └── proveedorPlan.MotivoCancelacion = "Cancelado - Fin de período"│
│  │                                                                      │
│  ├── NO genera notificaciones:                                          │
│  │   ├── ❌ NotificarVencimiento1Dia                                    │
│  │   ├── ❌ NotificarVencimiento5Dias                                   │
│  │   ├── ❌ ProcesarMoraYSuspension                                     │
│  │   └── ❌ ProcesarRenovacionesAutomaticas                             │
│  │                                                                      │
│  └── Razón: Usuario canceló intencionalmente, no tiene sentido          │
│     notificar ni ofrecer GRACE/SUSPENDED                                │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Campo CancelAtPeriodEnd

```
CancelAtPeriodEnd (bool):
  → true = Suscripción Culqi eliminada, plan activo hasta FechaFin
  → false = Suscripción Culqi activa, se cobrará en FechaProximoCobro
  → Se setea en: CancelAutoRenewCommandHandler
  → Se lee en: PlanExpirationService.ProcesarCancelacionesAlFinPeriodo()
  → Efecto: Cuando FechaFin pasa, plan va directo a CANCELLED (sin GRACE)
```

### Comportamiento Después de Cancelar

```
FLUJO COMPLETO:
  15 JUL: Usuario cancela renovación
    ├── CancelSubscriptionAsync → Culqi DELETE (permanente)
    ├── AutoRenovacion = false
    ├── CancelAtPeriodEnd = true
    └── Plan sigue ACTIVE con acceso normal

  15 JUL - 15 AGO: Plan sigue activo
    ├── Usuario puede usar todas las funciones
    ├── No se genera cobro en Culqi
    ├── No se envían notificaciones de vencimiento
    └── Badge cambia: "Renovación cancelada hasta 15 AGO"

  15 AGO: FechaFin llega
    ├── PlanExpirationService detecta:
    │   CancelAtPeriodEnd=true && FechaFin < now
    ├── Estado = CANCELLED (directo, sin GRACE)
    ├── EsActual = false
    └── Usuario debe seleccionar nuevo plan del catálogo
```

### Diferencias con Otros Flujos

| Aspecto | Flujo 1 | Flujo 2/3 | Flujo 4 | Flujo 5 |
|---------|---------|-----------|---------|---------|
| **Estado inicial** | Sin plan | GRACE | ACTIVE | ACTIVE |
| **Operación** | Crear suscripción | Reintentar pago | Cancelar + Nueva suscripción | Solo cancelar |
| **Culqi** | Crear | retryPayment | DELETE + CREATE | DELETE solamente |
| **Cobro** | Suscripción | Monto completo | Charge + Nueva suscripción | Ninguno |
| **FechaFin** | Nueva | Se mantiene | Nueva | Se mantiene |
| **Estado final** | PENDING | ACTIVE | PENDING | ACTIVE (hasta FechaFin) |
| **Después de FechaFin** | N/A | N/A | N/A | CANCELLED directo |
| **CancelAtPeriodEnd** | false | false | false | true |

---

## Webhooks - CulqiWebhookController.cs

### subscription.created
```csharp
case "subscription.created":
    // Setear FechaProximoCobro inicial desde next_billing_date
    if (data.Metadata?.TryGetValue("next_billing_date", out var nextBillingStr) == true
        && long.TryParse(nextBillingStr, out var nextBillingTimestamp))
    {
        proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(nextBillingTimestamp);
    }
    else if (data.NextBillingDate.HasValue)
    {
        proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(data.NextBillingDate.Value);
    }
    break;
```

### subscription.updated
```csharp
case "subscription.updated":
    // SOLO actualizar FechaProximoCobro (billing anchor)
    // NUNCA tocar FechaFin (es nuestra lógica de acceso)
    if (data.Metadata?.TryGetValue("next_billing_date", out var nextBillingStr) == true
        && long.TryParse(nextBillingStr, out var nextBillingTimestamp))
    {
        proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(nextBillingTimestamp);
    }
    else if (data.NextBillingDate.HasValue)
    {
        proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(data.NextBillingDate.Value);
    }
    break;
```

### subscription.deleted
```csharp
case "subscription.deleted":
    // Cancelación de renovación (Flujo 5)
    // Culqi eliminó la suscripción → plan se mantiene activo hasta FechaFin
    proveedorPlan.AutoRenovacion = false;
    proveedorPlan.CancelAtPeriodEnd = true;  // No setting Estado=CANCELLED now
    proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;
    proveedorPlan.MotivoCancelacion = "Cancelado en Culqi";
    
    // NO setear Estado=CANCELLED aquí
    // PlanExpirationService lo hará cuando FechaFin pase
    break;
```

### charge.succeeded
```csharp
case "charge.succeeded":
    // Buscar ProveedorPlan por metadata proveedor_id (unificado)
    var proveedorPlan = await FindProveedorPlanForCharge(data);
    
    if (proveedorPlan != null)
    {
        // Crear NUEVO PagoPlan con charge ID real (ch_001, ch_002...)
        var pagoPlan = new PagoPlan
        {
            IdProveedorPlan = proveedorPlan.IdProveedorPlan,
            Monto = proveedorPlan.IdPlanTarifaNavigation.Precio,
            CulqiChargeId = data.Id,  // charge ID REAL
            Estado = PAGADO
        };
        await _pagoPlanRepository.AddAsync(pagoPlan);
        
        // Actualizar ProveedorPlan
        proveedorPlan.Estado = ACTIVE;
        if (data.NextBillingDate.HasValue)
        {
            proveedorPlan.FechaFin = data.NextBillingDate;
            proveedorPlan.FechaProximoCobro = data.NextBillingDate;
        }
        proveedorPlan.GracePeriodHasta = null;
    }
    break;
```
### charge.failed
```csharp
case "charge.failed":
    // Buscar ProveedorPlan por metadata proveedor_id (unificado)
    var proveedorPlan = await FindProveedorPlanForCharge(data);
    
    if (proveedorPlan != null)
    {
        // Crear NUEVO PagoPlan RECHAZADO con charge ID real
        var pagoPlan = new PagoPlan
        {
            IdProveedorPlan = proveedorPlan.IdProveedorPlan,
            Monto = proveedorPlan.IdPlanTarifaNavigation.Precio,
            CulqiChargeId = data.Id,
            Estado = RECHAZADO
        };
        await _pagoPlanRepository.AddAsync(pagoPlan);
        
        // Cambiar ProveedorPlan a GRACE
        proveedorPlan.Estado = GRACE;
        proveedorPlan.GracePeriodHasta = DateTimeOffset.UtcNow.AddDays(5);
    }
    break;
```---

## Estados del Plan

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         ESTADOS DEL PLAN                                │
└─────────────────────────────────────────────────────────────────────────┘

                    ┌─────────────┐
                    │  PENDING    │ ← Recién creado, esperando webhook
                    └──────┬──────┘
                           │
              ┌────────────┴────────────┐
              │                         │
              ▼                         ▼
       ┌─────────────┐          ┌─────────────┐
       │   ACTIVE    │          │   GRACE     │ ← FechaFin llegó sin pago
       └──────┬──────┘          └──────┬──────┘
              │                         │
              │        ┌────────────────┤
              │        │                │
              ▼        ▼                ▼
       ┌─────────────┐          ┌─────────────┐
       │ CANCELLED   │          │ PAST_DUE    │ ← 5 días sin pagar en GRACE
       └─────────────┘          └──────┬──────┘
                                       │
                                       ▼
                                ┌─────────────┐
                                │ SUSPENDED   │ ← Plan suspendido
                                └─────────────┘

CancelAtPeriodEnd (no es estado, es flag que modifica comportamiento):
  ACTIVE + CancelAtPeriodEnd=true → Cuando FechaFin pase → CANCELLED directo (sin GRACE)
```

### Acciones por Estado

| Estado | Acción Disponible | Botón | Notas |
|--------|-------------------|-------|-------|
| `PENDING` | Esperando confirmación | Spinner | Esperando webhook charge.succeeded |
| `ACTIVE` | Ver plan, cambiar plan, cancelar renovación | Ver plan | Si `CancelAtPeriodEnd=true`, badge "Renovación cancelada" |
| `GRACE` | **Pagar plan** | Pagar plan - S/ XX | 5 días para pagar antes de SUSPENDED |
| `PAST_DUE` | **Pagar plan** | Pagar plan - S/ XX | Última oportunidad |
| `CANCELLED` | Seleccionar nuevo plan | Seleccionar Plan | Plan terminó, ir al catálogo |
| `SUSPENDED` | Seleccionar nuevo plan | Seleccionar Plan | Plan suspendido, ir al catálogo |

---

## Servicios Culqi Utilizados

### Frontend - CulqiService

```typescript
// culqi.service.ts

loadScript(): Promise<void>
// Carga CulqiJS v4 desde https://checkout.culqi.com/js/v4

openCheckout(config): Promise<CulqiCheckoutResult>
// Abre checkout de Culqi
// Retorna: { type: 'card' | 'order', id: string }
//   - type: 'card' → Culqi.token.id (tkn_xxxxx) → Suscripción con Customer+Card+Subscription
//   - type: 'order' → Culqi.order.id (ype_xxxxx) → Cargo único con Charge
```

### Backend - ICulqiService

```csharp
// CulqiService.cs

// Clientes
Task<CulqiCustomerResponse> CreateCustomerAsync(CulqiCreateCustomerRequest request);
Task<CulqiCustomerResponse?> GetCustomerAsync(string customerId);

// Planes
Task<CulqiPlanResponse> CreatePlanAsync(CulqiCreatePlanRequest request);
Task<CulqiPlanResponse?> GetPlanAsync(string planId);

// Suscripciones
Task<CulqiSubscriptionResponse> CreateSubscriptionAsync(CulqiCreateSubscriptionRequest request);
Task<CulqiSubscriptionResponse?> GetSubscriptionAsync(string subscriptionId);
Task<bool> CancelSubscriptionAsync(string subscriptionId);

// Tarjetas
Task<CulqiCardResponse?> GetCardAsync(string customerId);
Task<CulqiCardResponse> CreateCardAsync(string customerId, string tokenId);
Task<bool> DeleteCardAsync(string cardId);

// Cargos
Task<CulqiChargeResponse> CreateChargeAsync(CulqiCreateChargeRequest request);
```

---

## Endpoints API

| Endpoint | Método | Descripción | Handler |
|----------|--------|-------------|---------|
| `/api/ProveedorPlan/checkout` | POST | Checkout inicial | `CheckoutPlanCommandHandler` |
| `/api/ProveedorPlan/retry-payment` | POST | Reintentar pago | `RetryPaymentPlanCommandHandler` |
| `/api/ProveedorPlan/calculate-proration` | POST | Calcular prorrateo | `CalculateProrationQueryHandler` |
| `/api/ProveedorPlan/change-plan` | POST | Cambiar plan | `ChangePlanCommandHandler` |
| `/api/ProveedorPlan/cancel-auto-renew/{id}` | POST | Cancelar renovación | `CancelAutoRenewCommandHandler` |
| `/api/ProveedorPlan/current/{idProveedor}` | GET | Plan actual | `GetCurrentProveedorPlanQuery` |

---

## Constantes de Negocio

```csharp
// Estados de ProveedorPlan
Constants.ESTADO_PROV_PLAN.PENDING    // "PENDING"
Constants.ESTADO_PROV_PLAN.ACTIVE     // "ACTIVE"
Constants.ESTADO_PROV_PLAN.GRACE      // "GRACE"
Constants.ESTADO_PROV_PLAN.PAST_DUE   // "PAST_DUE"
Constants.ESTADO_PROV_PLAN.CANCELLED  // "CANCELLED"
Constants.ESTADO_PROV_PLAN.SUSPENDED  // "SUSPENDED"

// Estados de Pago
Constants.ESTADO_PAGO.Pagado      // "01"
Constants.ESTADO_PAGO.Pendiente   // "02"
Constants.ESTADO_PAGO.Rechazado   // "03"
Constants.ESTADO_PAGO.Parcial     // "04"

// Métodos de Pago
Constants.METODO_PAGO.Efectivo       // "02"
Constants.METODO_PAGO.Transferencia  // "03"
Constants.METODO_PAGO.Yape          // "04"
Constants.METODO_PAGO.Plin          // "05"
```

---

## Notas Técnicas

1. **Culqi maneja el cobro** - Solo registramos el pago, Culqi cobra automáticamente via suscripción
2. **Tarjeta = Token** - El token de CulqiJS se convierte en tarjeta en el backend
3. **Reemplazo, no actualización** - Siempre crear nueva → eliminar anterior
4. **Suscripción vinculada** - La tarjeta se asocia a la suscripción para cobros futuros
5. **Webhook confirmation** - El backend registra el pago, Culqi envía webhook para confirmar
6. **Separación de fechas** - `FechaFin` = acceso (nuestra lógica), `FechaProximoCobro` = billing (Culqi)
7. **Cambio de plan = Cancelar + Nueva** - Nunca actualizar suscripción existente, crear nueva
8. **CancelAtPeriodEnd** - Flag que indica que la suscripción Culqi fue eliminada pero el plan sigue activo hasta FechaFin. Cuando FechaFin pase, el plan va directo a CANCELLED sin GRACE ni notificaciones. Solo `PlanExpirationService` puede setear CANCELLED.

---

**Última Actualización**: 2026-08-28
**Versión**: 4.2


