# Documentación Técnica - Módulo Planes SaaS (Billing)

> Esta documentación es para el equipo de desarrollo **Frontend**.
> Contiene los endpoints, DTOs y flujos del sistema de planes de proveedores con Culqi.

---

## Índice

1. [Visión General](#1-visión-general)
2. [Arquitectura de Servicios](#2-arquitectura-de-servicios)
3. [API Endpoints](#3-api-endpoints)
4. [DTOs y Respuestas](#4-dtos-y-respuestas)
5. [Flujo de Checkout (Frontend)](#5-flujo-de-checkout-frontend)
6. [Guía de Integración Frontend](#6-guía-de-integración-frontend)
7. [Webhooks y Notificaciones](#7-webhooks-y-notificaciones)
8. [Estados y Ciclos de Vida](#8-estados-y-ciclos-de-vida)
9. [Casos de Error](#9-casos-de-error)

---

## 1. Visión General

El módulo de **Planes SaaS** permite a los proveedores de canchas contratar planes de suscripción mensual/anual con pagos automáticos via Culqi.

### Características

- Catálogo de planes configurables con tarifas y características
- Checkout con Yape/Plin/Tarjetas via Culqi
- Renovación automática
- Periodo de gracia (Grace Period) ante fallos
- Notificaciones automáticas por email
- Background jobs para gestión de vencimiento

---

## 2. Arquitectura de Servicios

El módulo se compone de **3 servicios** independientes:

| Servicio | Responsable | Endpoints Clave |
|----------|-------------|----------------|
| **Plane** | Catálogo de planes, tarifas y características | `GET /api/Plane/list` |
| **ProveedorPlan** | Suscripciones activas del proveedor | `GET /api/ProveedorPlan/current/{idProveedor}`, `POST /checkout`, `POST /change-plan`, `POST /cancel-auto-renew`, `POST /retry-payment` |
| **PagoPlan** | Historial de pagos del proveedor | `GET /api/PagoPlan/payments/{idProveedor}` |

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND                                │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐ │
│  │ Catálogo     │  │ Checkout     │  │ Panel del Proveedor   │ │
│  │ PlaneService │  │ Culqi        │  │ ProveedorPlanService │ │
│  └──────────────┘  └──────────────┘  │ PagoPlanService      │ │
│                                      └──────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
                                │ HTTP
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      BACKEND API                                │
│  ┌────────────────┐  ┌──────────────────┐  ┌────────────────┐ │
│  │ PlaneController│  │ProveedorPlanCtrl │  │ PagoPlanCtrl   │ │
│  │ GET /list      │  │ GET /current/{id}│  │ GET /payments  │ │
│  │                │  │ POST /checkout   │  │                │ │
│  │                │  │ POST /cancel-... │  │                │ │
│  │                │  │ POST /retry-...  │  │                │ │
│  └────────────────┘  └──────────────────┘  └────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│              INFRASTRUCTURE                                   │
│  - CulqiService (CreateSubscription, CreateCustomer)        │
│  - PlanExpirationService (Background Job - cada 24h)        │
│  - NotificacionService (Email)                          │
│  - CulqiWebhookController (subscription.* events)        │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. API Endpoints

### 3.1 Service: Plane (Catálogo)

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| GET | `/api/Plane/list` | Obtener todos los planes activos con tarifas y características | `ResponseDto<IEnumerable<ListPlaneDto>>` |
| GET | `/api/Plane/{id}` | Obtener plan por ID | `ResponseDto<GetPlaneDto>` |
| POST | `/api/Plane` | Crear plan (admin) | `ResponseDto<GetPlaneDto>` |
| PUT | `/api/Plane` | Actualizar plan (admin) | `ResponseDto<GetPlaneDto>` |
| DELETE | `/api/Plane/{id}` | Eliminar plan (admin) | `ResponseDto` |

### 3.2 Service: ProveedorPlan (Suscripciones)

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| GET | `/api/ProveedorPlan/current/{idProveedor}` | Plan actual del proveedor | `ResponseDto<GetProveedorPlanCurrentDto>` |
| POST | `/api/ProveedorPlan/checkout` | Iniciar compra de plan | `ResponseDto<CheckoutResponseDto>` |
| POST | `/api/ProveedorPlan/change-plan` | Cambiar plan (prorrateo automático via Culqi) | `ResponseDto<ChangePlanResponseDto>` |
| POST | `/api/ProveedorPlan/cancel-auto-renew/{idProveedorPlan}` | Cancelar renovación automática y suscripción en Culqi | `ResponseDto` |
| POST | `/api/ProveedorPlan/retry-payment` | Reintentar pago fallido | `ResponseDto` |
| POST | `/api/ProveedorPlan/list` | Listar suscripciones de un proveedor | `ResponseDto<IEnumerable<ListProveedorPlanDto>>` |
| POST | `/api/ProveedorPlan/search` | Buscar suscripciones con filtros | `ResponseDto<SearchResultDto<SearchProveedorPlanDto>>` |

### 3.3 Service: PagoPlan (Historial de Pagos)

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| GET | `/api/PagoPlan/payments/{idProveedor}` | Historial de pagos del proveedor | `ResponseDto<List<GetPagoPlanDto>>` |
| POST | `/api/PagoPlan/list` | Listar pagos de un proveedor | `ResponseDto<IEnumerable<ListPagoPlanDto>>` |
| POST | `/api/PagoPlan/search` | Buscar pagos con filtros | `ResponseDto<SearchResultDto<SearchPagoPlanDto>>` |

### 3.4 Webhooks

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/culqi/webhook` | Recepción de eventos Culqi |
| GET | `/api/culqi/webhook/test` | Test de webhook |

---

## 4. DTOs y Respuestas

### 4.1 Plane - ListPlaneDto

**Endpoint**: `GET /api/Plane/list`

```typescript
interface ListPlaneDto {
  codigo: string;
  nombre: string;
  descripcion: string | null;
  ordenVisual: number | null;
  planCaracteristicas: PlanCaracteristicaDto[];
  planTarifa: GetPlanTarifaDto[];
}

interface PlanCaracteristicaDto {
  idPlane: number;
  descripcion: string | null;
  orden: number;
}

interface GetPlanTarifaDto {
  idPlanTarifa: number;
  idPlane: number;
  codigo: string;
  nombre: string | null;
  precio: number;
  moneda: string;
  duracionDias: number;
  porcentajeDescuento: number | null;
  tipoCobro: string;
  permiteAutoRenovacion: boolean | null;
  activo: boolean;
}
```

**Ejemplo de respuesta**:

```json
{
  "isValid": true,
  "messages": [],
  "data": [
    {
      "codigo": "BASIC",
      "nombre": "Plan Básico",
      "descripcion": "Ideal para empezar",
      "ordenVisual": 1,
      "planCaracteristicas": [
        { "idPlane": 1, "descripcion": "Hasta 2 canchas", "orden": 1 },
        { "idPlane": 1, "descripcion": "Soporte por email", "orden": 2 }
      ],
      "planTarifa": [
        {
          "idPlanTarifa": 1,
          "idPlane": 1,
          "codigo": "MENSUAL",
          "nombre": "Mensual",
          "precio": 49.90,
          "moneda": "PEN",
          "duracionDias": 30,
          "porcentajeDescuento": null,
          "tipoCobro": "MENSUAL",
          "permiteAutoRenovacion": true,
          "activo": true
        },
        {
          "idPlanTarifa": 2,
          "idPlane": 1,
          "codigo": "ANUAL",
          "nombre": "Anual",
          "precio": 479.00,
          "moneda": "PEN",
          "duracionDias": 365,
          "porcentajeDescuento": 20,
          "tipoCobro": "ANUAL",
          "permiteAutoRenovacion": true,
          "activo": true
        }
      ]
    }
  ]
}
```

### 4.2 ProveedorPlan - GetProveedorPlanCurrentDto

**Endpoint**: `GET /api/ProveedorPlan/current/{idProveedor}`

```typescript
interface GetProveedorPlanCurrentDto {
  idProveedorPlan: number;
  idProveedor: number;
  idPlane: number;
  idPlanTarifa: number;
  fechaInicio: string;        // ISO 8601
  fechaFin: string;           // ISO 8601
  fechaProximoCobro: string | null;  // ISO 8601
  estado: string;             // PENDING, ACTIVE, GRACE, SUSPENDED, CANCELLED
  autoRenovacion: boolean;
  esActual: boolean;
  culqiSubscriptionId: string | null;
  culqiCustomerId: string | null;
  gracePeriodHasta: string | null;   // ISO 8601
  fechaCancelacion: string | null;   // ISO 8601
  motivoCancelacion: string | null;
  activo: boolean;

  // Datos del plan
  plan: {
    idPlane: number;
    codigo: string;
    nombre: string;
    descripcion: string | null;
    ordenVisual: number | null;
    activo: boolean;
  };

  // Datos de la tarifa
  planTarifas: {
    idPlanTarifa: number;
    idPlane: number;
    codigo: string;
    nombre: string | null;
    precio: number;
    moneda: string;
    duracionDias: number;
    porcentajeDescuento: number | null;
    tipoCobro: string;
    permiteAutoRenovacion: boolean | null;
    activo: boolean;
  };

  // Características del plan
  planCaracteristicas: PlanCaracteristicaDto[] | null;

  // Límites del plan
  limites: PlanLimiteDto[] | null;
}

interface PlanLimiteDto {
  idPlane: number;
  codigo: string;
  valor: number;
}
```

**Ejemplo de respuesta**:

```json
{
  "isValid": true,
  "messages": [],
  "data": {
    "idProveedorPlan": 10,
    "idProveedor": 42,
    "idPlane": 2,
    "idPlanTarifa": 3,
    "fechaInicio": "2026-01-01T00:00:00Z",
    "fechaFin": "2026-01-31T00:00:00Z",
    "fechaProximoCobro": "2026-01-31T00:00:00Z",
    "estado": "ACTIVE",
    "autoRenovacion": true,
    "esActual": true,
    "culqiSubscriptionId": "sub_test_abc123",
    "culqiCustomerId": "cus_test_xyz789",
    "gracePeriodHasta": null,
    "fechaCancelacion": null,
    "motivoCancelacion": null,
    "activo": true,
    "plan": {
      "idPlane": 2,
      "codigo": "PREMIUM",
      "nombre": "Plan Premium",
      "descripcion": "Todo lo que necesitas para crecer",
      "ordenVisual": 2,
      "activo": true
    },
    "planTarifas": {
      "idPlanTarifa": 3,
      "idPlane": 2,
      "codigo": "MENSUAL",
      "nombre": "Mensual",
      "precio": 99.00,
      "moneda": "PEN",
      "duracionDias": 30,
      "porcentajeDescuento": null,
      "tipoCobro": "MENSUAL",
      "permiteAutoRenovacion": true,
      "activo": true
    },
    "planCaracteristicas": [
      { "idPlane": 2, "descripcion": "Hasta 10 canchas", "orden": 1 },
      { "idPlane": 2, "descripcion": "Soporte prioritario", "orden": 2 },
      { "idPlane": 2, "descripcion": "Reportes avanzados", "orden": 3 }
    ],
    "limites": [
      { "idPlane": 2, "codigo": "MAX_CANCHAS", "valor": 10 },
      { "idPlane": 2, "codigo": "MAX_RESERVAS_DIA", "valor": 50 }
    ]
  }
}
```

### 4.3 ProveedorPlan - CheckoutResponseDto

**Endpoint**: `POST /api/ProveedorPlan/checkout`

```typescript
interface CheckoutResponseDto {
  idProveedorPlan: number;
  culqiSubscriptionId: string | null;
  referenceCode: string | null;
  monto: number;
  moneda: string;         // "PEN"
  estado: string;         // "PENDIENTE"
  fechaExpiracion: string | null;  // ISO 8601
  fechaProximoCobro: string | null; // ISO 8601
}
```

**Request**:

```typescript
interface CheckoutPlanDto {
  idProveedor: number;
  idPlane: number;
  idPlanTarifa: number;
  culqiToken: string | null;  // Token de Culqi (opcional si ya tiene tarjeta guardada)
  email: string;
}
```

**Ejemplo de respuesta**:

```json
{
  "isValid": true,
  "messages": [
    { "messageType": "Success", "text": "Suscripción iniciada. Espera la confirmación del webhook de Culqi." }
  ],
  "data": {
    "idProveedorPlan": 10,
    "culqiSubscriptionId": "sub_test_abc123",
    "referenceCode": "REF-12345",
    "monto": 99.00,
    "moneda": "PEN",
    "estado": "PENDIENTE",
    "fechaExpiracion": "2026-02-01T00:00:00Z",
    "fechaProximoCobro": "2026-02-01T00:00:00Z"
  }
}
```

### 4.3.1 ProveedorPlan - ChangePlanDto / ChangePlanResponseDto

**Endpoint**: `POST /api/ProveedorPlan/change-plan`

```typescript
interface ChangePlanDto {
  idProveedorPlan: number;
  idNuevoPlane: number;
  idNuevaPlanTarifa: number;
}

interface ChangePlanResponseDto {
  idProveedorPlan: number;
  idNuevoPlane: number;
  idNuevaPlanTarifa: number;
  culqiSubscriptionId: string | null;
  montoProrrateado: number;
  moneda: string;          // "PEN"
  estado: string;          // "ACTIVE"
  nuevaFechaFin: string | null;     // ISO 8601
  nuevaFechaProximoCobro: string | null; // ISO 8601
}
```

**Request**:

```json
{
  "idProveedorPlan": 10,
  "idNuevoPlane": 3,
  "idNuevaPlanTarifa": 5
}
```

**Ejemplo de respuesta**:

```json
{
  "isValid": true,
  "messages": [
    { "messageType": "Success", "text": "Plan cambiado exitosamente. Culqi aplicó el prorrateo correspondiente." }
  ],
  "data": {
    "idProveedorPlan": 10,
    "idNuevoPlane": 3,
    "idNuevaPlanTarifa": 5,
    "culqiSubscriptionId": "sub_test_abc123",
    "montoProrrateado": 149.00,
    "moneda": "PEN",
    "estado": "ACTIVE",
    "nuevaFechaFin": "2026-03-01T00:00:00Z",
    "nuevaFechaProximoCobro": "2026-03-01T00:00:00Z"
  }
}
```

### 4.4 PagoPlan - GetPagoPlanDto

**Endpoint**: `GET /api/PagoPlan/payments/{idProveedor}`

```typescript
interface GetPagoPlanDto {
  idPagoPlan: number;
  idProveedorPlan: number;
  monto: number;
  moneda: string | null;
  idMetodoPago: number;
  idEstadoPago: number;
  fechaPago: string | null;  // ISO 8601
  culqiSubscriptionId: string | null;
  codigoOperacion: string | null;
  respuestaGateway: string | null;
  estadoPago: string;        // Nombre del estado (Pagado, Pendiente, Rechazado)
}
```

**Ejemplo de respuesta**:

```json
{
  "isValid": true,
  "messages": [],
  "data": [
    {
      "idPagoPlan": 1,
      "idProveedorPlan": 10,
      "monto": 99.00,
      "moneda": "PEN",
      "idMetodoPago": 1,
      "idEstadoPago": 1,
      "fechaPago": "2026-01-01T10:30:00Z",
      "culqiSubscriptionId": "sub_test_abc123",
      "codigoOperacion": "REF-12345",
      "respuestaGateway": null,
      "estadoPago": "Pagado"
    },
    {
      "idPagoPlan": 2,
      "idProveedorPlan": 10,
      "monto": 99.00,
      "moneda": "PEN",
      "idMetodoPago": 1,
      "idEstadoPago": 2,
      "fechaPago": null,
      "culqiSubscriptionId": "sub_test_abc123",
      "codigoOperacion": null,
      "respuestaGateway": null,
      "estadoPago": "Pendiente"
    }
  ]
}
```

### 4.5 ProveedorPlan - ListProveedorPlanDto

**Endpoint**: `POST /api/ProveedorPlan/list` (body: `int idProveedor`)

```typescript
interface ListProveedorPlanDto {
  idProveedor: number;
  idPlane: number;
  idPlanTarifa: number;
  fechaInicio: string;        // ISO 8601
  fechaFin: string;           // ISO 8601
  fechaProximoCobro: string | null;
  estado: string;
  autoRenovacion: boolean;
  esActual: boolean;
  culqiSubscriptionId: string | null;
  culqiCustomerId: string | null;
  gracePeriodHasta: string | null;
  fechaCancelacion: string | null;
  motivoCancelacion: string | null;
}
```

### 4.6 ProveedorPlan - SearchProveedorPlanDto

**Endpoint**: `POST /api/ProveedorPlan/search`

```typescript
interface SearchProveedorPlanDto {
  idProveedorPlan: number | null;
  idProveedor: number;
  idPlane: number;
  idPlanTarifa: number;
  fechaInicio: string;
  fechaFin: string;
  fechaProximoCobro: string | null;
  estado: string;
  autoRenovacion: boolean;
  esActual: boolean;
  culqiSubscriptionId: string | null;
  culqiCustomerId: string | null;
  gracePeriodHasta: string | null;
  fechaCancelacion: string | null;
  motivoCancelacion: string | null;
}

interface SearchProveedorPlanFilterDto {
  fechaDesde: string | null;   // ISO 8601
  fechaHasta: string | null;   // ISO 8601
  idProveedorPlan: number | null;
  activo: boolean | null;
}
```

### 4.7 ResponseDto (Wrapper Genérico)

Todas las respuestas envuelven los datos en un `ResponseDto<T>`:

```typescript
interface ResponseDto<T> {
  isValid: boolean;
  messages: ApplicationMessageDto[];
  data: T | null;
}

interface ApplicationMessageDto {
  messageType: string;  // "Success", "Error", "Warning", "Info"
  text: string;
}
```

---

## 5. Flujo de Checkout (Frontend)

```
┌─────────────────────────────────────────────────────────────────┐
│                     FLUJO DE CHECKOUT                           │
└─────────────────────────────────────────────────────────────────┘

1. CATÁLOGO
   ┌─────────────────────────────────────────────────────────────┐
   │ GET /api/Plane/list                                         │
   │                                                           │
   │ Response: ResponseDto<IEnumerable<ListPlaneDto>>           │
   │ - Cada plan incluye planCaracteristicas[] y planTarifa[] │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
2. SELECCIONAR PLAN Y TARIFA
   ┌─────────────────────────────────────────────────────────────┐
   │ El frontend muestra los planes con sus tarifas              │
   │ El usuario selecciona un plan y una tarifa                  │
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
   │    Culqi.settings({ amount: monto * 100, currency: 'PEN'});│
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
   │ Body: CheckoutPlanDto                                     │
   │ {                                                         │
   │   "idProveedor": 42,                                      │
   │   "idPlane": 2,                                           │
   │   "idPlanTarifa": 3,                                      │
   │   "culqiToken": "tok_xxx",                                 │
   │   "email": "proveedor@email.com"                           │
   │ }                                                         │
   │                                                           │
   │ Response: ResponseDto<CheckoutResponseDto>                │
   │ {                                                         │
   │   "isValid": true,                                        │
   │   "data": {                                               │
   │     "idProveedorPlan": 10,                                │
   │     "culqiSubscriptionId": "sub_xxx",                     │
   │     "referenceCode": "REF-12345",                        │
   │     "monto": 99.00,                                      │
   │     "moneda": "PEN",                                      │
   │     "estado": "PENDIENTE",                                │
   │     "fechaExpiracion": "2026-02-01T00:00:00Z",           │
   │     "fechaProximoCobro": "2026-02-01T00:00:00Z"          │
   │   }                                                       │
   │ }                                                         │
   └─────────────────────────────────────────────────────────────┘
                              │
                              ▼
5. CONFIRMACIÓN POR WEBHOOK (Automático)
   ┌─────────────────────────────────────────────────────────────┐
   │ Culqi envía webhook: POST /api/culqi/webhook                 │
   │                                                           │
   │ Backend procesa:                                          │
   │ - Busca ProveedorPlan por culqiSubscriptionId             │
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
   │ Response: ResponseDto<GetProveedorPlanCurrentDto>         │
   │ - Incluye plan, planTarifas, planCaracteristicas, limites │
   └─────────────────────────────────────────────────────────────┘
```

---

## 5.1 Flujo de Cambio de Plan (Frontend)

```
┌─────────────────────────────────────────────────────────────────┐
│                    FLUJO DE CAMBIO DE PLAN                       │
└─────────────────────────────────────────────────────────────────┘

1. OBTENER PLAN ACTUAL
   ┌─────────────────────────────────────────────────────────────┐
   │ GET /api/ProveedorPlan/current/{idProveedor}              │
   │                                                           │
   │ Verificar: estado == "ACTIVE"                             │
   └─────────────────────────────────────────────────────────────┘
                               │
                               ▼
2. SELECCIONAR NUEVO PLAN Y TARIFA
   ┌─────────────────────────────────────────────────────────────┐
   │ GET /api/Plane/list (si aún no se tiene)                   │
   │                                                           │
   │ El usuario selecciona nuevo plan y tarifa                  │
   └─────────────────────────────────────────────────────────────┘
                               │
                               ▼
3. CONFIRMAR CAMBIO DE PLAN
   ┌─────────────────────────────────────────────────────────────┐
   │ POST /api/ProveedorPlan/change-plan                       │
   │ Body: ChangePlanDto                                       │
   │ {                                                         │
   │   "idProveedorPlan": 10,                                  │
   │   "idNuevoPlane": 3,                                      │
   │   "idNuevaPlanTarifa": 5                                   │
   │ }                                                         │
   │                                                           │
   │ Response: ResponseDto<ChangePlanResponseDto>              │
   │ {                                                         │
   │   "isValid": true,                                        │
   │   "data": {                                               │
   │     "idProveedorPlan": 10,                                │
   │     "idNuevoPlane": 3,                                    │
   │     "idNuevaPlanTarifa": 5,                               │
   │     "montoProrrateado": 149.00,                          │
   │     "estado": "ACTIVE",                                   │
   │     "nuevaFechaFin": "2026-03-01T00:00:00Z",            │
   │     "nuevaFechaProximoCobro": "2026-03-01T00:00:00Z"   │
   │   }                                                       │
   │ }                                                         │
   └─────────────────────────────────────────────────────────────┘
                               │
                               ▼
4. CULQi APLICA PRORRATEO (Automático)
   ┌─────────────────────────────────────────────────────────────┐
   │ Culqi calcula la diferencia de días restantes del ciclo    │
   │ actual y cobra el prorrateo automáticamente                │
   │                                                           │
   │ - Upgrade: cobra diferencia proporcional                  │
   │ - Downgrade: genera crédito para próximo cobro            │
   └─────────────────────────────────────────────────────────────┘
```

---

## 6. Guía de Integración Frontend

### 6.1 Servicio: Plane (Catálogo)

```javascript
// Obtener lista de planes con tarifas y características
async function getPlanes() {
  const response = await fetch('/api/Plane/list', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const result = await response.json();
  return result.data; // Array de ListPlaneDto
}
```

### 6.2 Servicio: ProveedorPlan (Suscripciones)

```javascript
// Obtener plan actual del proveedor
async function getMiPlan(idProveedor) {
  const response = await fetch(`/api/ProveedorPlan/current/${idProveedor}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const result = await response.json();
  return result.data; // GetProveedorPlanCurrentDto
}

// Checkout - Iniciar compra de plan
async function checkoutPlan(checkoutData) {
  const response = await fetch('/api/ProveedorPlan/checkout', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(checkoutData)
  });
  const result = await response.json();
  return result; // ResponseDto<CheckoutResponseDto>
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
async function reintentarPago(idProveedorPlan) {
  const response = await fetch('/api/ProveedorPlan/retry-payment', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      idProveedorPlan: idProveedorPlan
    })
  });
  const result = await response.json();
  return result;
}

// Cambiar plan (Culqi aplica prorrateo automático)
async function cambiarPlan(changePlanData) {
  const response = await fetch('/api/ProveedorPlan/change-plan', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(changePlanData)
  });
  const result = await response.json();
  return result; // ResponseDto<ChangePlanResponseDto>
}
```

### 6.3 Servicio: PagoPlan (Historial de Pagos)

```javascript
// Obtener historial de pagos del proveedor
async function getHistorialPagos(idProveedor) {
  const response = await fetch(`/api/PagoPlan/payments/${idProveedor}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const result = await response.json();
  return result.data; // Array de GetPagoPlanDto
}
```

### 6.4 Checkout con Culqi

```javascript
// Paso 1: Integrar CulqiJS en el HTML
// <script src="https://checkout.culqi.com/js/v4"></script>

// Paso 2: Configurar Culqi
Culqi.publicKey = 'pk_test_TU_CLAVE_PUBLICA';

function abrirCheckout(monto, email, idPlan, idProveedor, idPlanTarifa) {
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
        idPlanTarifa: idPlanTarifa
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
    // Mostrar mensaje: "Suscripción iniciada. Recibirás un correo de confirmación."
    console.log('Pago iniciado:', result.data);
  } else {
    // Mostrar error
    console.error('Error:', result.messages);
  }
}
```

### 6.5 Componentes UI Recomendados

```jsx
// Pantalla: Catálogo de Planes
const PlanCatalogo = () => {
  const [planes, setPlanes] = useState([]);

  useEffect(() => {
    loadPlanes();
  }, []);

  const loadPlanes = async () => {
    const data = await getPlanes();
    setPlanes(data);
    // Cada plan ya incluye planCaracteristicas[] y planTarifa[]
  };

  return (
    <div className="planes-grid">
      {planes.map(plan => (
        <PlanCard
          key={plan.codigo}
          plan={plan}
          tarifas={plan.planTarifa}
          caracteristicas={plan.planCaracteristicas}
          onSelect={(tarifa) => abrirCheckout(
            tarifa.precio,
            email,
            plan.idPlane,
            idProveedor,
            tarifa.idPlanTarifa
          )}
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
      <PlanActualCard
        plan={plan.plan}
        tarifa={plan.planTarifas}
        fechaInicio={plan.fechaInicio}
        fechaFin={plan.fechaFin}
        estado={plan.estado}
        autoRenovacion={plan.autoRenovacion}
        caracteristicas={plan.planCaracteristicas}
        limites={plan.limites}
      />
      <HistorialPagosList pagos={historial} />
      {plan.estado === 'GRACE' && (
        <ReintentarPagoButton
          onClick={() => reintentarPago(plan.idProveedorPlan)}
        />
      )}
      {plan.autoRenovacion && (
        <CancelarRenovacionButton
          onClick={() => cancelarAutoRenew(plan.idProveedorPlan)}
        />
      )}
      {plan.estado === 'ACTIVE' && (
        <CambiarPlanButton
          onClick={() => seleccionarNuevoPlan(plan.idProveedorPlan)}
        />
      )}
    </div>
  );
};

// Pantalla: Cambio de Plan
const CambiarPlan = () => {
  const [idProveedorPlan, setIdProveedorPlan] = useState(null);
  const [nuevoPlan, setNuevoPlan] = useState(null);
  const [nuevaTarifa, setNuevaTarifa] = useState(null);

  const handleConfirmarCambio = async () => {
    const result = await cambiarPlan({
      idProveedorPlan: idProveedorPlan,
      idNuevoPlane: nuevoPlan.idPlane,
      idNuevaPlanTarifa: nuevaTarifa.idPlanTarifa
    });

    if (result.isValid) {
      // Mostrar: "Plan cambiado exitosamente. Culqi aplicó el prorrateo."
      console.log('Nuevo monto:', result.data.montoProrrateado);
    } else {
      // Mostrar error
      console.error('Error:', result.messages);
    }
  };

  return (
    <div>
      <SelectorPlanes onSelect={(plan, tarifa) => {
        setNuevoPlan(plan);
        setNuevaTarifa(tarifa);
      }} />
      <button onClick={handleConfirmarCambio}>
        Confirmar Cambio de Plan
      </button>
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
| `charge.failed` | Pasar a GRACE + Notificar |
| `order.status.changed` (paid) | Activar plan |
| `order.status.changed` (expired) | Pasar a GRACE |
| `subscription.created` | Log de nueva suscripción |
| `subscription.updated` | Actualizar `FechaProximoCobro` |
| `subscription.deleted` | Cancelar `AutoRenovacion` |

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
| PlanExpirationService | Cada 24h | Verificar vencimientos, notificar, suspender |

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

CANCELLED (Cancelado por el proveedor)
     │
     └── [Nuevo checkout] ──→ PENDING → ACTIVE

SUSPENDED (Suspendido por mora)
     │
     └── [Nuevo checkout] ──→ PENDING → ACTIVE

ACTIVE ──→ [Cambio de plan] ──→ ACTIVE (prorrateo via Culqi)
                │
                ├── [Upgrade a plan superior] ──→ Cobra diferencia prorrateada
                └── [Downgrade a plan inferior] ──→ Crédito para próximo cobro
```

### Significado de Estados

| Estado | Significado | Acceso a Plataforma |
|--------|-------------|-------------------|
| PENDING | Esperando confirmación de pago | Limitado |
| ACTIVE | Plan activo y vigente | Completo |
| GRACE | Pago fallido, 5 días para regularizar | Completo |
| SUSPENDED | Suspendido por mora | Bloqueado |
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
| `pago no encontrado` | SubscriptionId no existe | Verificar en panel Culqi |
| `firma inválida` | Webhook no es de Culqi | Ignorado |
| `webhook duplicado` | Reintento de Culqi | Ya procesado (ignorar) |

### 9.3 Errores en Retry Payment

| Código | Causa | Acción |
|--------|------|--------|
| `suscripción no encontrada` | ID inválido | Verificar ID |
| `no en estado GRACE` | Plan no está en mora | No permitir reintento |
| `error Culqi` | Error en el cobro | Mostrar mensaje |

### 9.4 Errores en Change Plan

| Código | Causa | Acción |
|--------|------|--------|
| `suscripción no encontrada` | ID inválido | Verificar ID |
| `suscripción no está activa` | Plan cancelado o suspendido | Solo se puede cambiar en estado ACTIVE |
| `no tiene ID de Culqi` | Sin suscripción en Culqi | Hacer checkout primero |
| `nueva tarifa no encontrada` | ID de tarifa inválido | Verificar tarifa seleccionada |
| `error Culqi` | Error en la API de Culqi | Mostrar mensaje, reintentar |

---

## Checklist de Implementación Frontend

- [ ] Integrar CulqiJS v4 en el proyecto
- [ ] Implementar pantalla de catálogo de planes (`GET /api/Plane/list`)
- [ ] Implementar pantalla de tarifas (incluidas en ListPlaneDto.planTarifa[])
- [ ] Implementar checkout con Culqi (`POST /api/ProveedorPlan/checkout`)
- [ ] Implementar manejo del token de Culqi
- [ ] Implementar panel "Mi Plan" del proveedor (`GET /api/ProveedorPlan/current/{idProveedor}`)
- [ ] Implementar historial de pagos (`GET /api/PagoPlan/payments/{idProveedor}`)
- [ ] Implementar botón de cancelar renovación (`POST /api/ProveedorPlan/cancel-auto-renew/{idProveedorPlan}`)
- [ ] Implementar flujo de reintento de pago (estado GRACE) (`POST /api/ProveedorPlan/retry-payment`)
- [ ] Implementar pantalla de cambio de plan (`POST /api/ProveedorPlan/change-plan`)
- [ ] Configurar credenciales de prueba Culqi
- [ ] Probar flujo completo en ambiente de test

---

## URLs de Ambiente

| Ambiente | URL API | Culqi Panel |
|----------|---------|------------|
| Desarrollo | `http://localhost:5000` | https://integ-panel.culqi.com |
| Producción | `https://api.reservacanchas.com` | https://panel.culqi.com |

---

*Documento generado: 2026-05-01*
*Versión: 2.0 (Con 3 servicios: Plane, ProveedorPlan, PagoPlan)*
