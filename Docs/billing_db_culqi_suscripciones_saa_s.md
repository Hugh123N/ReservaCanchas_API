# Documentación Funcional - Billing SaaS con Culqi

## 1. Objetivo
Implementar un sistema de planes para proveedores bajo modelo SaaS con cobros recurrentes usando Culqi como pasarela de pagos.

El sistema principal administra canchas, reservas y usuarios. El módulo Billing administra:

- Catálogo de planes
- Suscripciones activas de proveedores
n- Cobros recurrentes
- Comprobantes
- Renovaciones automáticas
- Suspensión por mora
- Notificaciones por correo
- Límites de uso del plan

---

## 2. Responsabilidades

## Culqi se encargará de:
- Tokenización segura de tarjeta
- Checkout de pago inicial
- Cobros recurrentes automáticos (mensual/anual)
- Procesamiento bancario
- Eventos vía webhook
- Seguridad del medio de pago

## Nuestro sistema se encargará de:
- Mostrar planes en frontend
- Registrar suscripciones en base de datos
- Activar/desactivar acceso del proveedor
- Registrar pagos e historial
- Emitir comprobantes
- Enviar correos automáticos
- Aplicar límites del plan
- Suspender por falta de pago

---

## 3. Tablas Principales (BillingDB)

- Plan
- PlanTarifa
- PlanCaracteristica
- PlanLimite
- ProveedorPlan
- PagoPlan
- UsoPlan
- ComprobantePagoPlan

---

## 4. Flujo General

```text
Proveedor entra al portal
        ↓
Visualiza planes disponibles
        ↓
Selecciona plan mensual/anual
        ↓
Frontend abre Checkout Culqi
        ↓
Proveedor ingresa tarjeta
        ↓
Culqi procesa pago
        ↓
Webhook / respuesta OK
        ↓
Backend registra suscripción
        ↓
Registrar pago + comprobante
        ↓
Enviar correo confirmación
        ↓
Proveedor usa plataforma
```

---

## 5. Proceso Inicial de Compra

## Paso 1: Selección del plan
Frontend consulta API Billing:

- planes activos
- precio mensual
- precio anual
- características
- descuentos

## Paso 2: Checkout Culqi
Frontend abre Culqi Checkout con:

- monto
- moneda PEN
- descripción plan
- datos cliente

## Paso 3: Pago exitoso
Si Culqi responde OK:

Backend ejecuta:

1. Crear registro en `ProveedorPlan`
2. Estado = ACTIVE
3. FechaInicio = hoy
4. FechaFin = hoy + duración plan
5. Guardar `culqiSubscriptionId`
6. Crear `PagoPlan`
7. Crear `ComprobantePagoPlan`
8. Enviar correo con comprobante
9. Habilitar acceso al proveedor

---

## 6. Renovación Automática Mensual / Anual

Culqi realizará automáticamente el cobro en la fecha programada.

## Cuando Culqi cobre correctamente:
Webhook recibido:

- payment.success
- subscription.renewed

Backend ejecuta:

1. Registrar nuevo `PagoPlan`
2. Extender `fechaFin`
3. Actualizar `fechaProximoCobro`
4. Generar comprobante
5. Enviar correo de renovación exitosa
6. Mantener plan activo

---

## 7. Fallo en Cobro Automático

Webhook recibido:

- payment.failed

Backend ejecuta:

1. Registrar `PagoPlan` FAILED
2. Cambiar estado a `GRACE`
3. Guardar `gracePeriodHasta = fechaFin + 5 días`
4. Enviar correo indicando:
   - no se pudo cobrar
   - actualizar tarjeta
   - regularizar pago

---

## 8. Cambio de Tarjeta por Cliente

Cliente entra al portal:

- actualiza medio de pago en Culqi
- reintenta pago

Si pago exitoso:

1. Estado vuelve a ACTIVE
2. Extender vigencia
3. Registrar pago
4. Correo de confirmación

---

## 9. Suspensión por Mora

Si pasan 5 días desde vencimiento sin pago:

Job interno ejecuta:

1. Cambiar estado a EXPIRED o SUSPENDED
2. Deshabilitar acceso del proveedor
3. Bloquear creación de canchas/reservas administrativas
4. Enviar correo final de suspensión
5. Invitar a renovar plan

---

## 10. Jobs Internos Requeridos

## Job Diario 08:00 AM - Aviso de Vencimiento
Buscar planes que vencen mañana.

Acciones:
- enviar correo recordatorio

## Job Diario 09:00 AM - Revisión de Mora
Buscar planes en GRACE vencidos.

Acciones:
- suspender acceso
- enviar correo final

## Job Diario 10:00 AM - Auditoría Webhooks
Buscar pagos pendientes sin confirmación.

Acciones:
- consultar estado con Culqi

---

## 11. Estados Recomendados de ProveedorPlan

- PENDING
- ACTIVE
- GRACE
- PAST_DUE
- CANCELLED
- EXPIRED
- SUSPENDED

---

## 12. Reglas de Negocio

## ACTIVE
Proveedor usa plataforma normal.

## GRACE
Proveedor usa plataforma temporalmente, pendiente regularización.

## EXPIRED / SUSPENDED
Sin acceso operativo.

## CANCELLED
No renovar siguiente ciclo.

---

## 13. Integración Frontend

## Pantalla de planes
Mostrar:
- nombre plan
- precio mensual
- precio anual
- ahorro anual
- características
- botón contratar

## Panel proveedor
Mostrar:
- plan actual
- fecha renovación
- estado
- historial pagos
- cambiar tarjeta
- cancelar renovación automática

---

## 14. Integración Backend

Endpoints sugeridos:

- GET /plans
- POST /subscription/checkout
- POST /webhooks/culqi
- GET /subscription/current
- GET /subscription/payments
- POST /subscription/cancel-auto-renew
- POST /subscription/retry-payment

---

## 15. Seguridad

Nunca guardar:

- número completo tarjeta
- CVV
- datos sensibles bancarios

Solo guardar IDs/token devueltos por Culqi.

---

## 16. KPI SaaS Futuro

- MRR (ingreso mensual recurrente)
- ARR
- churn rate
- clientes activos
- morosos
- renovaciones exitosas
- upgrades/downgrades

---

## 17. Decisión Estratégica

Culqi cobra.
Nuestro sistema gobierna acceso, planes y negocio.

Esto reduce riesgo técnico y acelera salida a producción.

