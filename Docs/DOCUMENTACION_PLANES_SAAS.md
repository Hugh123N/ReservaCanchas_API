# Documentación Técnica - Módulo Planes SaaS (Billing)

> Esta documentación es para el equipo de desarrollo **Frontend**.
> Contiene los endpoints, DTOs y flujos del sistema de planes de proveedores con Culqi.

---

## Índice

1. [Visión General](#1-visión-general)
2. [Arquitectura de Servicios](#2-arquitectura-de-servicios)
3. [API Endpoints](#3-api-endpoints)
5. [Flujo de Checkout (Frontend)](#5-flujo-de-checkout-frontend)
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

### 3.4 Service: ComprobantePagoPlan (Comprobantes de Pago)

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| POST | `/api/ComprobantePagoPlan` | Crear comprobante de pago | `ResponseDto<GetComprobantePagoPlanDto>` |
| PUT | `/api/ComprobantePagoPlan` | Actualizar comprobante | `ResponseDto<GetComprobantePagoPlanDto>` |
| DELETE | `/api/ComprobantePagoPlan/{id}` | Eliminar comprobante | `ResponseDto` |
| GET | `/api/ComprobantePagoPlan/{id}` | Obtener comprobante por ID | `ResponseDto<GetComprobantePagoPlanDto>` |
| POST | `/api/ComprobantePagoPlan/list` | Listar comprobantes de un pago | `ResponseDto<IEnumerable<ListComprobantePagoPlanDto>>` |
| POST | `/api/ComprobantePagoPlan/search` | Buscar comprobantes con filtros | `ResponseDto<SearchResultDto<SearchComprobantePagoPlanDto>>` |

### 3.5 Webhooks

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/culqi/webhook` | Recepción de eventos Culqi |
| GET | `/api/culqi/webhook/test` | Test de webhook |

---

## 3.6 Modelo de Entidades

### Entidad: Plan (Catálogo)

```sql
IdPlane              INT IDENTITY PK
Codigo               VARCHAR(50) UNIQUE    -- Código del plan
Nombre               VARCHAR(200)          -- Nombre del plan
Descripcion          TEXT                  -- Descripción detallada
OrdenVisual          INT                   -- Orden de visualización en el catálogo
Activo               BIT                   -- Activo/Inactivo

-- Relaciones:
PlanCaracteristica[] <- Características del plan
PlanTarifa[]         <- Tarifas disponibles (mensual, anual, etc.)
PlanLimite[]         <- Límites del plan (max canchas, max operadores, etc.)
ProveedorPlan[]      <- Suscripciones de proveedores
```

### Entidad: PlanTarifa (Tarifas)

```sql
IdPlanTarifa         INT IDENTITY PK
IdPlane              INT FK -> Plan
Codigo               VARCHAR(50)          -- Código de la tarifa
Nombre               VARCHAR(200)         -- Nombre (ej: "Mensual", "Anual")
Precio               DECIMAL(10,2)        -- Precio de la tarifa
Moneda               CHAR(3)              -- "PEN" (soles)
DuracionDias         INT                  -- Duración en días (30=mensual, 365=anual)
PorcentajeDescuento  DECIMAL(5,2)         -- Descuento aplicado (opcional)
TipoCobro            VARCHAR(50)          -- Tipo de cobro (recurrente, único)
PermiteAutoRenovacion BIT                 -- Si permite renovación automática
Activo               BIT
```

### Entidad: PlanCaracteristica (Características)

```sql
IdPlanCaracteristica INT IDENTITY PK
IdPlane              INT FK -> Plan
Descripcion          VARCHAR(500)         -- Característica (ej: "Hasta 5 canchas")
Orden                INT                  -- Orden de visualización
Activo               BIT
```

### Entidad: PlanLimite (Límites del Plan)

```sql
IdPlanLimite         INT IDENTITY PK
IdPlane              INT FK -> Plan
Codigo               VARCHAR(50)          -- Código del límite
Valor                INT                  -- Valor numérico del límite
Activo               BIT
```

### Entidad: ProveedorPlan (Suscripciones)

```sql
IdProveedorPlan      INT IDENTITY PK
IdProveedor          INT FK -> Proveedor
IdPlane              INT FK -> Plan
IdPlanTarifa         INT FK -> PlanTarifa
FechaInicio          DATETIMEOFFSET       -- Inicio de la suscripción
FechaFin             DATETIMEOFFSET       -- Fin de la suscripción
FechaProximoCobro    DATETIMEOFFSET       -- Próxima fecha de cobro
Estado               VARCHAR(20)          -- PENDING|ACTIVE|GRACE|SUSPENDED|CANCELLED
AutoRenovacion       BIT                  -- Renovación automática habilitada
EsActual             BIT                  -- Si es el plan actual del proveedor
CulqiSubscriptionId  VARCHAR(100)         -- ID de suscripción en Culqi
CulqiCustomerId      VARCHAR(100)         -- ID de cliente en Culqi
GracePeriodHasta     DATETIMEOFFSET       -- Fecha límite del periodo de gracia
FechaCancelacion     DATETIMEOFFSET       -- Fecha de cancelación (si aplica)
MotivoCancelacion    VARCHAR(500)         -- Razón de cancelación
-- Audit: userNameCreate, createDate, activo
```

### Entidad: PagoPlan (Pagos)

```sql
IdPagoPlan           INT IDENTITY PK
IdProveedorPlan      INT FK -> ProveedorPlan
Monto                DECIMAL(10,2)        -- Monto pagado
Moneda               CHAR(3)              -- "PEN"
IdMetodoPago         INT FK -> MetodoPago
IdEstadoPago         INT FK -> EstadoPago
FechaPago            DATETIMEOFFSET       -- Fecha del pago
CulqiChargeId        VARCHAR(100)         -- ID del cargo en Culqi
CodigoOperacion      VARCHAR(100)         -- Código de operación del gateway
RespuestaGateway     TEXT                 -- Respuesta raw de Culqi (JSON)
-- Audit: userNameCreate, createDate, activo
```

### Entidad: ComprobantePagoPlan (Comprobantes)

```sql
IdComprobantePagoPlan INT IDENTITY PK
IdPagoPlan            INT FK -> PagoPlan
TipoComprobante       VARCHAR(20)          -- Boleta | Factura | Recibo
Serie                 VARCHAR(10)          -- Serie del comprobante
Numero                VARCHAR(20)          -- Número correlativo
RazonSocial           VARCHAR(200)         -- Razón social (Factura)
Ruc                   VARCHAR(11)          -- RUC (Factura)
Direccion             VARCHAR(500)         -- Dirección fiscal
UrlPdf                VARCHAR(500)         -- URL del PDF
UrlXml                VARCHAR(500)         -- URL del XML
FechaEmision          DATETIMEOFFSET       -- Fecha de emisión
EstadoSunat           VARCHAR(20)          -- Estado ante SUNAT
Hash                  VARCHAR(500)         -- Hash de seguridad
-- Audit: userNameCreate, createDate, activo
```

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

## 7. Webhooks y Notificaciones

### 7.1 Eventos de Culqi

| Evento | Acción en Backend |
|-------|------------------|
| `charge.succeeded` | Activar plan (ACTIVE) + Notificar |
| `charge.failed` | Pasar a GRACE + Notificar |
| `order.status.changed` state=`paid` o `paid_out` | Activar plan |
| `order.status.changed` state=`expired` o `deleted` | Pasar a GRACE |
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
| PlanExpirationService | Cada 24h | Gestión completa de vencimientos y renovaciones |

#### Tareas del PlanExpirationService

| Tarea | Descripción |
|-------|-------------|
| `NotificarVencimiento1Dia` | Envía email a proveedores cuyo plan vence mañana |
| `NotificarVencimiento5Dias` | Envía email a proveedores en estado GRACE por 5 días |
| `ProcesarMoraYSuspension` | Cambia a SUSPENDED los planes en GRACE que ya expiraron |
| `ProcesarRenovacionesAutomaticas` | Procesa renovaciones vía Culqi para planes con AutoRenovación |

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
