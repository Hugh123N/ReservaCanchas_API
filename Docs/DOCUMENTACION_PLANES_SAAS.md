# Documentación Técnica - Módulo Planes SaaS (Billing)

> Esta documentación es para el equipo de desarrollo backend y frontend.
> Contiene la implementación del sistema de planes de proveedores con Culqi.

---

## Índice

1. [Visión General](#1-visión-general)
2. [Arquitectura del Módulo](#2-arquitectura-del-módulo)
3. [Entidades y Base de Datos](#3-entidades-y-base-de-datos)
4. [API Endpoints](#4-api-endpoints)
5. [Flujo de Checkout (Frontend)](#5-flujo-de-checkout-frontend)
6. [Guía de Integración Frontend](#6-guía-de-integración-frontend)
7. [Webhooks y Notificaciones](#7-webhooks-y-notificaciones)
8. [Estados y Ciclos de Vida](#8-estados-y-ciclos-de-vida)
9. [Casos de Error](#9-casos-de-error)

---

## 1. Visión General

El módulo de **Planes SaaS** permite a los proveedores de canchas contratar planes de suscripción mensual/anual con pagos automáticos via Culqi.

### Características

- Catálogo de planes configurables
- Checkout con Yape/Plin/Tarjetas
- Renovación automática
- Periodo de gracia (Grace Period) ante fallos
- Notificaciones automáticas por email
- Background jobs para gestión de vencimiento

---

## 2. Arquitectura del Módulo

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND                                │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐ │
│  │ Catálogo     │  │ Checkout     │  │ Panel del Proveedor   │ │
│  │ Planes      │  │ Culqi        │  │ (Mi Plan, Historial)   │ │
│  └──────────────┘  └──────────────┘  └──────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ HTTP
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      BACKEND API                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ ProveedorPlanController                                 │ │
│  │ - GET /current/{idProveedor}                       │ │
│  │ - POST /checkout                                  │ │
│  │ - GET /payments/{idProveedor}                    │ │
│  │ - POST /cancel-auto-renew                        │ │
│  │ - POST /retry-payment                         │ │
│  └──────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────���─────────────┐
│              APPLICATION LAYER                                │
│  ProveedorPlanApplication                                    │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│              DOMAIN LAYER (CQRS)                                │
│  Commands: CreateCheckout, CancelAutoRenew, RetryPayment     │
│  Queries: GetCurrentPlan, GetPaymentsHistory               │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│              INFRASTRUCTURE                                   │
│  - CulqiService (CreateCharge)                             │
│  - PlanExpirationService (Background Job)                │
│  - NotificacionService (Email)                          │
│  - CulqiWebhookController (Webhook)                    │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Entidades y Base de Datos

### Plane (Catálogo de Planes)

```csharp
public partial class Plane
{
    public int IdPlane { get; set; }
    public string Codigo { get; set; }       // "BASIC", "PREMIUM", "ENTERPRISE"
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int? OrdenVisual { get; set; }
    public bool Activo { get; set; }
}
```

### PlanTarifa (Tarifas de un Plan)

```csharp
public partial class PlanTarifa
{
    public int IdPlanTarifa { get; set; }
    public int IdPlane { get; set; }
    public string Codigo { get; set; }           // "MENSUAL", "ANUAL"
    public string? Nombre { get; set; }           // "Mensual", " Anual"
    public decimal Precio { get; set; }            // 99.00
    public string Moneda { get; set; }             // "PEN"
    public int DuracionDias { get; set; }           // 30 o 365
    public decimal? PorcentajeDescuento { get; set; } // 20 (20% anual)
    public string TipoCobro { get; set; }          // "MENSUAL" o "ANUAL"
    public bool? PermiteAutoRenovacion { get; set; }
    public bool Activo { get; set; }
}
```

### ProveedorPlan (Suscripción Activa)

```csharp
public partial class ProveedorPlan
{
    public int IdProveedorPlan { get; set; }
    public int IdProveedor { get; set; }
    public int IdPlane { get; set; }
    public int IdPlanTarifa { get; set; }
    public DateTimeOffset FechaInicio { get; set; }
    public DateTimeOffset FechaFin { get; set; }
    public DateTimeOffset? FechaProximoCobro { get; set; }
    public string Estado { get; set; }              // PENDING, ACTIVE, GRACE, SUSPENDED, EXPIRED
    public bool AutoRenovacion { get; set; }
    public bool EsActual { get; set; }
    public string? CulqiSubscriptionId { get; set; }  // ID del cargo en Culqi
    public string? CulqiCustomerId { get; set; }
    public DateTimeOffset? GracePeriodHasta { get; set; }
    public DateTimeOffset? FechaCancelacion { get; set; }
    public string? MotivoCancelacion { get; set; }
    public string UserNameCreate { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public bool Activo { get; set; }
}
```

### PagoPlan (Historial de Pagos)

```csharp
public partial class PagoPlan
{
    public int IdPagoPlan { get; set; }
    public int IdProveedorPlan { get; set; }
    public decimal Monto { get; set; }
    public string? Moneda { get; set; }
    public int IdMetodoPago { get; set; }
    public int IdEstadoPago { get; set; }
    public DateTimeOffset? FechaPago { get; set; }
    public string? CulqiChargeId { get; set; }
    public string? CodigoOperacion { get; set; }
    public string? RespuestaGateway { get; set; }
    public bool Activo { get; set; }
}
```

---

## 4. API Endpoints

### Catálogo de Planes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Plane` | Obtener todos los planes activos |
| GET | `/api/Plane/{id}` | Obtenerplan por ID |
| GET | `/api/Plane/selectcombo` | Planes para dropdown |
| GET | `/api/Plane/tarifas/{idPlan}` | Tarifas de un plan |

### Gestión de Suscripciones

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|--------------|
| GET | `/api/ProveedorPlan/current/{idProveedor}` | Plan actual del proveedor | Proveedor |
| POST | `/api/ProveedorPlan/checkout` | Iniciar compra de plan | Proveedor |
| GET | `/api/ProveedorPlan/payments/{idProveedor}` | Historial de pagos | Proveedor |
| POST | `/api/ProveedorPlan/cancel-auto-renew/{idProveedorPlan}` | Cancelar renovación | Proveedor |
| POST | `/api/ProveedorPlan/retry-payment` | Reintentar pago fallido | Proveedor |

### Webhooks

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/culqi/webhook` | Recepcón de eventos Culqi |

---

## 5. Flujo de Checkout (Frontend)

```
┌─────────────────────────────────────────────────────────────────┐
│                     FLUJO DE CHECKOUT                           │
└─────────────────────────────────────────────────────────────────┘

1. CATÁLOGO
   ┌─────────────────────────────────────────────────────────────┐
   │ GET /api/Plane/selectcombo                                  │
   │                                                           │
   │ Response:                                                 │
   │ {                                                       │
   │   "data": [                                              │
   │     { "idPlane": 1, "nombre": "Basic", "descripcion": "..."},│
   │     { "idPlane": 2, "nombre": "Premium", "descripcion": "..."}│
   │   ]                                                     │
   │ }                                                       │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
2. SELECCIONAR TARIFA
   ┌─────────────────────────────────────────────────────────────┐
   │ GET /api/Plane/tarifas/{idPlan}                             │
   │                                                           │
   │ Response:                                                 │
   │ {                                                       │
   │   "data": [                                              │
   │     { "idPlanTarifa": 1, "nombre": "Mensual", "precio": 99},│
   │     { "idPlanTarifa": 2, "nombre": "Anual", "precio": 990,  │
   │       "porcentajeDescuento": 20 }                          │
   │   ]                                                     │
   │ }                                                       │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
3. CHECKOUT CULQI (Frontend)
   ┌─────────────────────────────────────────────────────────────┐
   │ 1. Frontend carga CulqiJS                                  │
   │    <script src="https://checkout.culqi.com/js/v4"></script> │
   │                                                           │
   │ 2. Configurar y abrir checkout                            │
   │    Culqi.publicKey = 'pk_test_XXX';                       │
   │    Culqi.settings({ amount: 9900, currency: 'PEN' ...}); │
   │    Culqi.open();                                          │
   │                                                           │
   │ 3. Capturar token (.callback de Culqi)                   │
   │    function culqi() {                                     │
   │      if (Culqi.token) {                                   │
   │        token = Culqi.token.id;                             │
   │        // Llamar al backend                                │
   │      }                                                   │
   │    }                                                    │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
4. PROCESAR PAGO EN BACKEND
   ┌─────────────────────────────────────────────────────────────┐
   │ POST /api/ProveedorPlan/checkout                           │
   │ Body:                                                     │
   │ {                                                         │
   │   "idProveedor": 1,                                       │
   │   "idPlane": 1,                                           │
   │   "idPlanTarifa": 1,                                      │
   │   "culqiToken": "tok_xxx",                                 │
   │   "email": "proveedor@email.com"                           │
   │ }                                                         │
   │                                                           │
   │ Response (202 Accepted - espera webhook):                   │
   │ {                                                         │
   │   "data": {                                               │
   │     "idProveedorPlan": 10,                                │
   │     "culqiChargeId": "chr_xxx",                           │
   │     "referenceCode": "REF-12345",                        │
   │     "monto": 99.00,                                      │
   │     "estado": "PENDIENTE"                                 │
   │   },                                                     │
   │   "isValid": true,                                        │
   │   "messages": ["Pago iniciado..."]                       │
   │ }                                                         │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
5. CONFIRMACIÓN POR WEBHOOK (Automático)
   ┌─────────────────────────────────────────────────────────────┐
   │ Culqi envía webhook: POST /api/culqi/webhook                 │
   │                                                           │
   │ Backend procesa:                                          │
   │ - Busca proveedorPlan por culqiChargeId                   │
   │ - Actualiza estado a ACTIVE                                │
   │ - Registra pago exitoso                                   │
   │ - Envía correo de confirmación                           │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
6. CONSULTA DE PLAN ACTUAL
   ┌─────────────────────────────────────────────────────────────┐
   │ GET /api/ProveedorPlan/current/{idProveedor}              │
   │                                                           │
   │ Response:                                                 │
   │ {                                                         │
   │   "data": {                                              │
   │     "idProveedorPlan": 10,                               │
   │     "idPlane": 1,                                       │
   │     "nombrePlan": "Premium",                             │
   │     "nombreTarifa": "Mensual",                           │
   │     "fechaInicio": "2026-01-01",                       │
   │     "fechaFin": "2026-01-31",                         │
   │     "estado": "ACTIVE",                                │
   │     "autoRenovacion": true                              │
   │   }                                                      │
   │ }                                                         │
   └─────────────────────────────────────────────────────────────┘
```

---

## 6. Guía de Integración Frontend

### 6.1 Catálogo de Planes

```javascript
// Obtener lista de planes para mostrar en el catálogo
async function getPlanes() {
  const response = await fetch('/api/Plane/selectcombo', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const data = await response.json();
  return data.data;
}

// Obtener tarifas de un plan (para mostrar precios)
async function getTarifas(idPlan) {
  const response = await fetch(`/api/Plane/tarifas/${idPlan}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const data = await response.json();
  return data.data;
}
```

### 6.2 Checkout de Plan

```javascript
// Paso 1: Integrar CulqiJS en el HTML
// <script src="https://checkout.culqi.com/js/v4"></script>

// Paso 2: Configurar Culqi (en tu JavaScript)
Culqi.publicKey = 'pk_test_TU_CLAVE_PUBLICA';

function abrirCheckout(monto, email, idPlan, idProveedor) {
  Culqi.settings({
    title: 'ReservaCanchas - Plan Premium',
    currency: 'PEN',
    amount: monto * 100,  // En centavos
    description: 'Plan Premium Mensual',
    checkout: 'standard',
    mode: 'payment'
  });

  Culqi.open();

  // Escuchar el resultado
  window.culqi = function() {
    if (Culqi.token) {
      procesarPago({
        culqiToken: Culqi.token.id,
        email: email,
        idProveedor: idProveedor,
        idPlane: idPlan,
        idPlanTarifa: tarifaSeleccionada.idPlanTarifa
      });
    } else if (Culqi.error) {
      console.error('Error Culqi:', Culqi.error);
    }
  };
}

// Paso 3: Llamar al backend
async function procesarPago(checkoutData) {
  const response = await fetch('/api/ProveedorPlan/checkout', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(checkoutData)
  });

  const result = await response.json();

  if (result.isValid) {
    // Mostrar mensaje: "Pago iniciado. Recibirás un correo de confirmación."
    console.log('Pago iniciado:', result.data);
  } else {
    // Mostrar error
    console.error('Error:', result.messages);
  }
}
```

### 6.3 Panel del Proveedor

```javascript
// Obtener plan actual del proveedor
async function getMiPlan(idProveedor) {
  const response = await fetch(`/api/ProveedorPlan/current/${idProveedor}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const result = await response.json();
  return result.data;
}

// Obtener historial de pagos
async function getHistorialPagos(idProveedor) {
  const response = await fetch(`/api/ProveedorPlan/payments/${idProveedor}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const result = await response.json();
  return result.data;
}

// Cancelar renovación automática
async function cancelarAutoRenew(idProveedorPlan) {
  const response = await fetch(`/api/ProveedorPlan/cancel-auto-renew/${idProveedorPlan}`, {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const result = await response.json();
  return result;
}

// Reintentar pago (si está en GRACE)
async function reintentarPago(idProveedorPlan, culqiToken) {
  const response = await fetch('/api/ProveedorPlan/retry-payment', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      idProveedorPlan: idProveedorPlan,
      culqiToken: culqiToken
    })
  });
  const result = await response.json();
  return result;
}
```

### 6.4 Componentes UI Recomendados

```jsx
// Pantalla: Catálogo de Planes
const PlanCatalogo = () => {
  const [planes, setPlanes] = useState([]);
  const [tarifas, setTarifas] = useState({});

  useEffect(() => {
    loadPlanes();
  }, []);

  const loadPlanes = async () => {
    const data = await getPlanes();
    setPlanes(data);
    // Cargar tarifas de cada plan
    for (const plan of data) {
      const t = await getTarifas(plan.idPlane);
      setTarifas(prev => ({ ...prev, [plan.idPlane]: t }));
    }
  };

  return (
    <div className="planes-grid">
      {planes.map(plan => (
        <PlanCard
          key={plan.idPlane}
          plan={plan}
          tarifas={tarifas[plan.idPlane]}
          onSelect={(tarifa) => abrirCheckout(tarifa)}
        />
      ))}
    </div>
  );
};

// Pantalla: Mi Plan Actual
const MiPlan = () => {
  const [plan, setPlan] = useState(null);
  const [historial, setHistorial] = useState([]);

  useEffect(() => {
    const loadData = async () => {
      const p = await getMiPlan(idProveedor);
      const h = await getHistorialPagos(idProveedor);
      setPlan(p);
      setHistorial(h);
    };
    loadData();
  }, []);

  if (!plan) return <Spinner />;

  return (
    <div>
      <PlanActualCard plan={plan} />
      <HistorialPagosList pagos={historial} />
      {plan.estado === 'GRACE' && (
        <ReintentarPagoButton
          onClick={() => abrirCheckoutReintento(plan.idProveedorPlan)}
        />
      )}
      {plan.autoRenovacion && (
        <CancelarRenovacionButton
          onClick={() => cancelarAutoRenew(plan.idProveedorPlan)}
        />
      )}
    </div>
  );
};
```

---

## 7. Webhooks y Notificaciones

### 7.1 Eventos de Culqi

| Evento | Acción en Backend |
|-------|------------------|
| `charge.succeeded` | Activar plan (ACTIVE) + Notificar |
| `charge.failed` | Pasara GRACE + Notificar |
| `order.status.changed` (paid) | Activar plan |
| `order.status.changed` (expired) | Pasar a GRACE |

### 7.2 Notificaciones Automáticas

| Tipo | Trigger | Destinatario | Canal |
|-----|---------|-------------|-------|
| `PLAN_VENCIMIENTO_1_DIA` | 1 día antes de vencer | Proveedor | Email |
| `PLAN_VENCIMIENTO_5_DIAS` | 5 días en GRACE | Proveedor | Email |
| `PLAN_FALLO_PAGO` | Pago fallido | Proveedor | Email |
| `PLAN_RENOVACION` | Pago exitoso | Proveedor | Email |

### 7.3 Background Jobs

| Job | Frecuencia | Acción |
|-----|-----------|--------|
| PlanExpirationService | Cada 24h | Verificar vencimientos |

---

## 8. Estados y Ciclos de Vida

```
┌─────────────────────────────────────────────────────────────────┐
│                    CICLO DE VIDA DEL PLAN                        │
└─────────────────────────────────────────────────────────────────┘

[Pago iniciado]
     │
     ▼
PENDING (esperando webhook de Culqi)
     │
     ├── [Webhook charge.succeeded] ──→ ACTIVE
     │                                        │
     │    ┌────────────────────────────────────┴──────────┐
     │    │                                            │
     │    │ [Pago recurrente exitoso]                 │
     │    │         │                                 │
     │    │         ▼                                 │
     │    │    ACTIVE (renovado)                       │
     │    │         │                                 │
     │    │         │ [1 día antes de vencer]         │
     │    │         │         │                        │
     │    │         │         ▼                        │
     │    │         │    Notificación                │
     │    │         │    (VENCIMIENTO_1_DIA)         │
     │    │         │                                 │
     │    │         └─────────────────────────────────┘
     │    │
     │    └── [Webhook charge.failed] ──→ GRACE
     │                                        │
     │         ┌───────────────────────────────┘
     │         │
     ▼         ▼
GRACE (Periodo de gracia - 5 días)
     │
     ├── [Pago reintentado exitoso] ──→ ACTIVE
     │
     ├── [No pagar en 5 días] ──→ SUSPENDED
     │
     └── [5 días en GRACE] ──→ Notificación
                           (VENCIMIENTO_5_DIAS)

EXPIRED (Venció sin renovación automática)
     │
     └── [Nuevo checkout] ──→ PENDING → ACTIVE

SUSPENDED (Suspendido por mora)
     │
     └── [Nuevo checkout] ──→ PENDING → ACTIVE
```

### Significado de Estados

| Estado | Significado | Acceso a Plataforma |
|--------|-------------|-------------------|
| PENDING | Esperando confirmación de pago | Limitado |
| ACTIVE | Plan activo y vigente | Completo |
| GRACE | Pago fallido, 5 días para regularizar | Completo |
| SUSPENDED | Suspendido por mora | Bloqueado |
| EXPIRED | Plan vencido | Bloqueado |
| CANCELLED | Cancelado por el proveedor | Bloqueado |

---

## 9. Casos de Error

### 9.1 Errores en Checkout

| Código | Causa | Acción Recommended |
|--------|------|-------------------|
| `tarifa no encontrada` | ID de tarifa inválido | Verificar ID seleccionado |
| `proveedor no encontrado` | ID de proveedor inválido | Verificar sesión |
| `token inválido` | Token Culqi expirado | Generar nuevo token |
| `error Culqi` | Error de la pasarela | Mostrar mensaje de Culqi |

### 9.2 Errores en Webhook

| Código | Causa | Acción |
|--------|------|--------|
| `pago no encontrado` | ChargeId no existe | Verificar enpanel Culqi |
| `firma inválida` | Webhook no es de Culqi | Ignorado |
| `webhook duplicado` | Reintento de Culqi | Ya procesado (ignorar) |

### 9.3 Errores en Retry Payment

| Código | Causa | Acción |
|--------|------|--------|
| `suscripción no encontrada` | ID inválido | Verificar ID |
| `no en estado GRACE` | Plan no está en mora | No permitir reintento |
| `error Culqi` | Error en el cobro | Mostrar mensaje |

---

## Checklist de Implementación Frontend

- [ ] Integrar CulqiJS v4 en el proyecto
- [ ] Implementar pantalla de catálogo de planes
- [ ] Implementar pantalla de tarifas (mensual/anual)
- [ ] Implementar checkout con Culqi
- [ ] Implementar manejo del token de Culqi
- [ ] Implementar panel "Mi Plan" del proveedor
- [ ] Implementar historial de pagos
- [ ] Implementar botón de cancelar renovación
- [ ] Implementar flujo de reintento de pago (estado GRACE)
- [ ] Configurar credenciales de prueba Culqi
- [ ] Probar flujo completo en ambiente de test

---

## URLs de Ambiente

| Ambiente | URL API | Culqi Panel |
|----------|---------|------------|
| Desarrollo | `http://localhost:5000` | https://integ-panel.culqi.com |
| Producción | `https://api.reservacanchas.com` | https://panel.culqi.com |

---

*Documento generado: 2026-04-26*
*Versión: 1.0*