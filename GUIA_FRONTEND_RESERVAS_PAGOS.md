# 📱 GUÍA FRONTEND - SISTEMA DE RESERVAS Y PAGOS CON IZIPAY

## 📋 TABLA DE CONTENIDOS

1. [Introducción](#introducción)
2. [Métodos de Pago Disponibles](#métodos-de-pago-disponibles)
3. [Estados del Sistema](#estados-del-sistema)
4. [Endpoints API](#endpoints-api)
5. [Flujos Completos](#flujos-completos)
   - [Flujo 1: Reserva con Yape (Izipay)](#flujo-1-reserva-con-yape-izipay)
   - [Flujo 2: Reserva con Plin (Izipay)](#flujo-2-reserva-con-plin-izipay)
   - [Flujo 3: Reserva con Efectivo (Sin Adelanto)](#flujo-3-reserva-con-efectivo-sin-adelanto)
   - [Flujo 4: Reserva con Efectivo (Con Adelanto)](#flujo-4-reserva-con-efectivo-con-adelanto)
6. [Manejo de Errores](#manejo-de-errores)
7. [Ejemplos de Interfaz](#ejemplos-de-interfaz)

---

## INTRODUCCIÓN

Este documento describe cómo integrar el sistema de reservas de canchas con **Izipay** (pasarela de pago oficial) para Yape/Plin, y gestión manual de efectivo.

**Base URL:** `https://tu-api.com/api`

**Autenticación:** JWT Bearer Token (agregar en headers)

```javascript
headers: {
  'Authorization': 'Bearer YOUR_JWT_TOKEN',
  'Content-Type': 'application/json'
}
```

---

## MÉTODOS DE PAGO DISPONIBLES

| ID | Código | Método   | Estado     | Flujo                           |
|----|--------|----------|------------|---------------------------------|
| 4  | 04     | Yape     | ✅ Activo   | QR oficial vía Izipay           |
| 5  | 05     | Plin     | ✅ Activo   | QR oficial vía Izipay           |
| 2  | 02     | Efectivo | ✅ Activo   | Con/Sin adelanto (manual)       |

**Para obtener métodos de pago disponibles:**
```http
GET /api/MetodoPago/selectcombo
```

---

## ESTADOS DEL SISTEMA

### Estados de Reserva

| Código | Estado      | Color    | Descripción                    |
|--------|-------------|----------|--------------------------------|
| 01     | Pendiente   | 🟡 Yellow | Esperando confirmación de pago |
| 02     | Confirmado  | 🟢 Green  | Pago confirmado, cancha reservada |
| 03     | Cancelado   | 🔴 Red    | Reserva cancelada              |

### Estados de Pago

| Código | Estado     | Color    | Descripción                    |
|--------|------------|----------|--------------------------------|
| 01     | Pagado     | 🟢 Green  | Pago completado totalmente     |
| 02     | Pendiente  | 🟡 Yellow | Sin pagar aún                  |
| 03     | Rechazado  | 🔴 Red    | Pago rechazado o fallido       |
| 04     | Parcial    | 🟠 Orange | Con adelanto (solo efectivo)   |

---

## ENDPOINTS API

### 📍 Reservas

```http
POST   /api/Reserva                    # Crear reserva
GET    /api/Reserva/{id}               # Obtener reserva
POST   /api/Reserva/search             # Buscar reservas
```

### 💰 Pagos

```http
POST /api/Pago/confirmar               # Confirmar pago efectivo
POST /api/Pago/completar-pago          # Completar pago parcial (efectivo)
GET  /api/Pago/{id}                    # Consultar estado de pago
```

### 🔔 Webhooks (Solo backend - no llamar desde frontend)

```http
POST /api/IzipayWebhook/notification   # Recibe notificaciones de Izipay
```

---

## FLUJOS COMPLETOS

---

## FLUJO 1: RESERVA CON YAPE (IZIPAY)

### 🎯 Caso de Uso
Cliente hace reserva online y paga con Yape escaneando QR **oficial** de Izipay.

### 📱 Interfaz de Usuario

**Pantalla 1: Selección de método de pago**
```
┌──────────────────────────────┐
│ Método de Pago               │
│                              │
│ ○ Efectivo                   │
│ ● Yape                       │
│ ○ Plin                       │
│                              │
│ [Continuar]                  │
└──────────────────────────────┘
```

### 📡 Implementación

#### **PASO 1: Crear Reserva**

```javascript
// Request
POST /api/Reserva
Content-Type: application/json
Authorization: Bearer {token}

{
  "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
  "idCancha": 5,
  "codigoMetodoPago": "04",  // 04 = Yape
  "fecha": "2025-11-01T00:00:00",
  "detalles": [
    {
      "horaInicio": "10:00:00",
      "horaFin": "12:00:00"
    }
  ],
  "monto": 50.00
}
```

```javascript
// Response (200 OK)
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Yape.",
  "data": {
    "reserva": {
      "idReserva": 123,
      "idEstadoReserva": 1  // PENDIENTE (esperando pago)
    },
    "pago": {
      "idPago": 456,
      "monto": 50.00,
      "idEstadoPago": 2  // PENDIENTE
    },
    // ✅ NUEVOS CAMPOS DE IZIPAY
    "izipayFormToken": "a1b2c3d4e5f6g7h8i9j0...",
    "izipayPaymentUrl": "https://secure.micuentaweb.pe/payment?formToken=xyz...",
    "izipayTransactionId": "TXN-ABC123",
    "metodoPago": "Yape",
    "montoFormateado": "50.00",
    "moneda": "PEN",
    "informacionAdicional": "Escanea el QR oficial de Yape..."
  }
}
```

**Frontend - Guardar datos:**
```javascript
const {
  reserva,
  pago,
  izipayFormToken,
  izipayPaymentUrl,
  izipayTransactionId
} = response.data;

// Guardar para polling posterior
localStorage.setItem('currentPaymentId', pago.idPago);
localStorage.setItem('currentReservaId', reserva.idReserva);
```

#### **PASO 2: Mostrar Interfaz de Pago Izipay**

Tienes **3 opciones** para mostrar el pago:

##### **OPCIÓN A: Embedded Form (Recomendado)**

Carga el SDK de Izipay e incrusta el formulario en tu página:

```html
<!-- Cargar SDK de Izipay -->
<script src="https://static.micuentaweb.pe/static/js/krypton-client/V4.0/stable/kr-payment-form.min.js"></script>
<link rel="stylesheet" href="https://static.micuentaweb.pe/static/js/krypton-client/V4.0/stable/kr-payment-form.min.css">

<!-- Contenedor del formulario -->
<div id="pago-container">
  <div class="kr-embedded" kr-form-token="{{izipayFormToken}}"></div>
</div>

<script>
// Configurar Izipay
KR.setFormConfig({
  formToken: '{{izipayFormToken}}',
  language: 'es-PE'
});

// Evento cuando el pago se completa
KR.onSubmit(function(paymentData) {
  // Izipay procesó el pago, esperar webhook
  // Mostrar pantalla de "Procesando..."
  showProcessingScreen();

  // Iniciar polling para verificar estado
  startPaymentPolling({{pago.idPago}});
});
</script>
```

##### **OPCIÓN B: Redirección**

Redirige al usuario a la página de pago de Izipay:

```javascript
// Redirigir directamente
window.location.href = response.data.izipayPaymentUrl;

// Izipay mostrará el QR y redirigirá de vuelta a tu app
// cuando el pago se complete
```

##### **OPCIÓN C: Modal/Popup**

Abre la página de Izipay en un modal o popup:

```javascript
const popup = window.open(
  response.data.izipayPaymentUrl,
  'IzipayPayment',
  'width=600,height=800'
);

// Escuchar mensaje de cierre
window.addEventListener('message', (event) => {
  if (event.data.type === 'payment-complete') {
    popup.close();
    showProcessingScreen();
    startPaymentPolling(pago.idPago);
  }
});
```

#### **PASO 3: Cliente Paga con Yape**

1. Cliente ve QR oficial de Izipay
2. Abre app Yape
3. Escanea QR → **App abre automáticamente** con monto pre-cargado
4. Confirma pago en la app
5. Yape notifica a Izipay → Izipay notifica a tu backend (webhook)

#### **PASO 4: Frontend Verifica el Pago**

**OPCIÓN A: Polling (Consultar cada 2-3 segundos)**

```javascript
function startPaymentPolling(idPago) {
  const maxAttempts = 60; // 3 minutos máximo
  let attempts = 0;

  const interval = setInterval(async () => {
    attempts++;

    try {
      const response = await fetch(`/api/Pago/${idPago}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });

      const data = await response.json();

      if (data.data.idEstadoPago === 1) {
        // ✅ PAGADO
        clearInterval(interval);
        showSuccessScreen(data.data);
      } else if (data.data.idEstadoPago === 3) {
        // ❌ RECHAZADO
        clearInterval(interval);
        showErrorScreen('Pago rechazado');
      } else if (attempts >= maxAttempts) {
        // ⏱️ TIMEOUT
        clearInterval(interval);
        showTimeoutScreen();
      }
    } catch (error) {
      console.error('Error al consultar pago:', error);
    }
  }, 3000); // Cada 3 segundos
}
```

**OPCIÓN B: WebSocket/SignalR (Tiempo real)**

Si implementas SignalR en el backend, puedes recibir notificación instantánea:

```javascript
// Conectar a SignalR
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/paymentHub")
  .build();

connection.on("PaymentStatusChanged", (pagoId, nuevoEstado) => {
  if (pagoId === currentPaymentId) {
    if (nuevoEstado === 1) {
      showSuccessScreen();
    } else if (nuevoEstado === 3) {
      showErrorScreen();
    }
  }
});

await connection.start();
```

#### **PASO 5: Mostrar Confirmación**

```javascript
function showSuccessScreen(pagoData) {
  const html = `
    <div class="success-screen">
      <div class="icon">✅</div>
      <h2>¡Reserva Confirmada!</h2>
      <p>Tu pago de <strong>S/ ${pagoData.monto.toFixed(2)}</strong> fue procesado exitosamente.</p>

      <div class="details">
        <p><strong>Código de operación:</strong> ${pagoData.codigoOperacion}</p>
        <p><strong>Fecha:</strong> ${formatDate(pagoData.createDate)}</p>
      </div>

      <button onclick="goToMyReservations()">Ver Mis Reservas</button>
    </div>
  `;

  document.getElementById('container').innerHTML = html;
}
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO** (código 02)
- ✅ Pago: **PAGADO** (código 01)

---

## FLUJO 2: RESERVA CON PLIN (IZIPAY)

### 🎯 Caso de Uso
**Idéntico a Yape**, solo cambia el `codigoMetodoPago`.

### 📡 Implementación

```javascript
POST /api/Reserva

{
  "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
  "idCancha": 5,
  "codigoMetodoPago": "05",  // 05 = Plin
  "fecha": "2025-11-01T00:00:00",
  "detalles": [...],
  "monto": 50.00
}
```

El resto del flujo es **100% idéntico** a Yape.

---

## FLUJO 3: RESERVA CON EFECTIVO (SIN ADELANTO)

### 🎯 Caso de Uso
Operador registra reserva, cliente llega y paga todo en efectivo.

### 📱 Interfaz de Usuario (Panel Operador)

```
┌──────────────────────────────┐
│ Nueva Reserva - Efectivo     │
│                              │
│ Cliente: Juan Pérez          │
│ Cancha: Cancha 2             │
│ Fecha: 01/11/2025            │
│ Hora: 14:00 - 16:00          │
│ Monto: S/ 80.00              │
│                              │
│ [Registrar Reserva]          │
└──────────────────────────────┘
```

### 📡 Implementación

#### **PASO 1: Operador crea reserva**

```javascript
POST /api/Reserva

{
  "idUsuario": "789e4567-e89b-12d3-a456-426614174000",
  "idCancha": 2,
  "codigoMetodoPago": "02",  // Efectivo
  "fecha": "2025-11-01T00:00:00",
  "detalles": [...],
  "monto": 80.00
  // NO enviar montoAdelanto
}
```

```javascript
// Response
{
  "isSuccess": true,
  "data": {
    "reserva": {
      "idReserva": 124,
      "idEstadoReserva": 1  // PENDIENTE
    },
    "pago": {
      "idPago": 457,
      "monto": 80.00,
      "montoAdelanto": 0,
      "montoPendiente": 80.00,
      "idEstadoPago": 2  // PENDIENTE
    },
    "informacionAdicional": "Cliente debe pagar S/ 80.00 en efectivo al llegar..."
  }
}
```

#### **PASO 2: Cliente llega y paga completo**

```javascript
POST /api/Pago/confirmar

{
  "idPago": 457,
  "codigoOperacion": "RECIBO-001"  // Número de recibo interno
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Pago confirmado exitosamente.",
  "data": {
    "idPago": 457,
    "idEstadoPago": 1  // ✅ PAGADO
  }
}
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO**
- ✅ Pago: **PAGADO**

---

## FLUJO 4: RESERVA CON EFECTIVO (CON ADELANTO)

### 🎯 Caso de Uso
Cliente da adelanto del 50% o más, reserva se confirma automáticamente.

### 📱 Interfaz de Usuario

```
┌──────────────────────────────┐
│ Nueva Reserva - Efectivo     │
│                              │
│ Cliente: María López         │
│ Monto Total: S/ 100.00       │
│                              │
│ ☑ Cliente da adelanto        │
│ Adelanto: [50.00_______]     │
│                              │
│ [Registrar Reserva]          │
└──────────────────────────────┘
```

### 📡 Implementación

#### **PASO 1: Crear reserva con adelanto**

```javascript
POST /api/Reserva

{
  "idUsuario": "abc-456-def",
  "idCancha": 3,
  "codigoMetodoPago": "02",
  "fecha": "2025-11-01T00:00:00",
  "detalles": [...],
  "monto": 100.00,
  "montoAdelanto": 50.00  // ← ADELANTO
}
```

```javascript
// Response
{
  "isSuccess": true,
  "data": {
    "reserva": {
      "idReserva": 125,
      "idEstadoReserva": 2  // ✅ CONFIRMADO (porque adelanto >= 50%)
    },
    "pago": {
      "idPago": 458,
      "monto": 100.00,
      "montoAdelanto": 50.00,
      "montoPendiente": 50.00,
      "idEstadoPago": 4  // 🟠 PARCIAL
    }
  }
}
```

#### **PASO 2: Cliente llega y completa pago**

```javascript
POST /api/Pago/completar-pago

{
  "idPago": 458,
  "montoRestante": 50.00,
  "numeroRecibo": "REC-003"
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Pago completado exitosamente.",
  "data": {
    "idPago": 458,
    "montoAdelanto": 100.00,  // 50 + 50
    "montoPendiente": 0,
    "idEstadoPago": 1  // ✅ PAGADO
  }
}
```

---

## MANEJO DE ERRORES

### Error 1: Cancha no disponible

```javascript
{
  "isSuccess": false,
  "message": "Ya existe una reserva para esta cancha en el horario seleccionado.",
  "data": null
}
```

### Error 2: Error al crear pago en Izipay

```javascript
{
  "isSuccess": false,
  "message": "Error al procesar pago con Izipay. Error: Connection timeout",
  "data": null
}
```

**Frontend:**
```javascript
if (!response.isSuccess) {
  showError(response.message);
  // Permitir reintentar
  enableRetryButton();
}
```

---

## EJEMPLOS DE INTERFAZ

### 🎨 Componente: Pago con Izipay (React)

```jsx
import { useState, useEffect } from 'react';

function IzipayPayment({ formToken, pagoId, onSuccess, onError }) {
  useEffect(() => {
    // Cargar SDK de Izipay
    const script = document.createElement('script');
    script.src = 'https://static.micuentaweb.pe/static/js/krypton-client/V4.0/stable/kr-payment-form.min.js';
    script.onload = () => initializeIzipay();
    document.body.appendChild(script);

    return () => document.body.removeChild(script);
  }, []);

  const initializeIzipay = () => {
    KR.setFormConfig({
      formToken: formToken,
      language: 'es-PE'
    });

    KR.onSubmit(() => {
      // Iniciar polling
      startPolling(pagoId, onSuccess, onError);
    });
  };

  return (
    <div className="izipay-payment">
      <h2>Completa tu pago</h2>
      <div className="kr-embedded" kr-form-token={formToken}></div>
    </div>
  );
}

function startPolling(pagoId, onSuccess, onError) {
  const interval = setInterval(async () => {
    try {
      const res = await fetch(`/api/Pago/${pagoId}`);
      const data = await res.json();

      if (data.data.idEstadoPago === 1) {
        clearInterval(interval);
        onSuccess(data.data);
      } else if (data.data.idEstadoPago === 3) {
        clearInterval(interval);
        onError('Pago rechazado');
      }
    } catch (err) {
      console.error(err);
    }
  }, 3000);
}
```

### 🎨 Componente: Estado del Pago

```jsx
function PaymentStatus({ estado }) {
  const estados = {
    1: { color: 'green', icon: '✅', text: 'Pagado' },
    2: { color: 'yellow', icon: '⏳', text: 'Pendiente' },
    3: { color: 'red', icon: '❌', text: 'Rechazado' },
    4: { color: 'orange', icon: '🟠', text: 'Parcial' }
  };

  const { color, icon, text } = estados[estado];

  return (
    <span className={`badge badge-${color}`}>
      {icon} {text}
    </span>
  );
}
```

---

## 📋 CHECKLIST DE IMPLEMENTACIÓN FRONTEND

### Página: Nueva Reserva con Yape/Plin

- [ ] Formulario para seleccionar cancha, fecha, hora
- [ ] Selector de método de pago (Yape, Plin, Efectivo)
- [ ] Llamada a POST /api/Reserva
- [ ] Recibir `izipayFormToken`, `izipayPaymentUrl`, `izipayTransactionId`
- [ ] Cargar SDK de Izipay
- [ ] Mostrar formulario embedded de Izipay
- [ ] Implementar polling para verificar estado del pago
- [ ] Mostrar pantalla de "Procesando pago..."
- [ ] Mostrar pantalla de éxito/error según resultado

### Página: Pago con Efectivo (Panel Operador)

- [ ] Checkbox "Cliente da adelanto"
- [ ] Input para monto de adelanto (validar <= monto total)
- [ ] Mostrar advertencia si adelanto < 50%
- [ ] Buscar reserva pendiente por ID
- [ ] Botón "Completar Pago" para adelantos parciales
- [ ] Input para número de recibo

---

**Última actualización:** 2025-10-31
**Versión:** 2.0 - Integración con Izipay
**Estado:** ✅ PRODUCCIÓN (Yape/Plin vía Izipay + Efectivo manual)
