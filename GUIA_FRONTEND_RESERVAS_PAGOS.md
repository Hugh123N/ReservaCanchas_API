# 📱 GUÍA FRONTEND - SISTEMA DE RESERVAS Y PAGOS

## 📋 TABLA DE CONTENIDOS

1. [Introducción](#introducción)
2. [Métodos de Pago Disponibles](#métodos-de-pago-disponibles)
3. [Estados del Sistema](#estados-del-sistema)
4. [Endpoints API](#endpoints-api)
5. [Flujos Completos](#flujos-completos)
   - [Flujo 1: Reserva con Yape](#flujo-1-reserva-con-yape)
   - [Flujo 2: Reserva con Plin](#flujo-2-reserva-con-plin)
   - [Flujo 3: Reserva con Efectivo (Sin Adelanto)](#flujo-3-reserva-con-efectivo-sin-adelanto)
   - [Flujo 4: Reserva con Efectivo (Con Adelanto)](#flujo-4-reserva-con-efectivo-con-adelanto)
   - [Flujo 5: Reserva con Transferencia](#flujo-5-reserva-con-transferencia-mercadopago)
6. [Manejo de Errores](#manejo-de-errores)
7. [Ejemplos de Interfaz](#ejemplos-de-interfaz)

---

## INTRODUCCIÓN

Este documento describe cómo integrar el sistema de reservas de canchas con diferentes métodos de pago desde el frontend.

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

| ID | Código | Método        | Estado     | Flujo                           |
|----|--------|---------------|------------|---------------------------------|
| 1  | 01     | Tarjeta       | 🚧 Futuro   | No implementado                 |
| 2  | 02     | Efectivo      | ✅ Activo   | Con/Sin adelanto                |
| 3  | 03     | Transferencia | 🚧 Futuro   | MercadoPago (placeholder)       |
| 4  | 04     | Yape          | ✅ Activo   | QR Code instantáneo             |
| 5  | 05     | Plin          | ✅ Activo   | QR Code instantáneo             |

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
PUT    /api/Reserva                    # Actualizar reserva
DELETE /api/Reserva/{id}               # Cancelar reserva
```

### 💰 Pagos

```http
POST /api/Pago/confirmar               # Confirmar pago (Yape/Plin/Efectivo completo)
POST /api/Pago/completar-pago          # Completar pago parcial (solo Efectivo con adelanto)
GET  /api/Pago/{id}                    # Consultar estado de pago
```

### 🏟️ Canchas

```http
GET  /api/Cancha/{id}                  # Obtener detalles de cancha
POST /api/Cancha/search                # Buscar canchas disponibles
```

---

## FLUJOS COMPLETOS

---

## FLUJO 1: RESERVA CON YAPE

### 🎯 Caso de Uso
Cliente hace reserva online y paga con Yape escaneando QR code.

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

**Pantalla 2: Mostrar QR y esperar confirmación**
```
┌──────────────────────────────┐
│ Escanea con Yape             │
│                              │
│   ┌──────────────┐           │
│   │              │           │
│   │   QR CODE    │           │
│   │              │           │
│   └──────────────┘           │
│                              │
│ Monto: S/ 50.00              │
│ Expira en: 14:32             │
│                              │
│ [Ya pagué - Confirmar]       │
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
  "idMetodoPago": 4,  // 4 = Yape
  "fecha": "2025-10-28T00:00:00",
  "horaInicio": "10:00:00",
  "horaFin": "12:00:00",
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
      "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
      "idCancha": 5,
      "fecha": "2025-10-28",
      "horaInicio": "10:00:00",
      "horaFin": "12:00:00",
      "monto": 50.00,
      "idEstadoReserva": 1  // PENDIENTE
    },
    "pago": {
      "idPago": 456,
      "idReserva": 123,
      "monto": 50.00,
      "montoAdelanto": 0,
      "montoPendiente": 50.00,
      "moneda": "PEN",
      "idMetodoPago": 4,
      "idEstadoPago": 2,  // PENDIENTE
      "codigoOperacion": null
    },
    "qrCodeBase64": "iVBORw0KGgoAAAANSUhEUgAA...",  // ← Imagen QR en Base64
    "qrText": "{\"tipo\":\"YAPE\",\"telefono\":\"901269594\",\"monto\":\"50.00\",...}",
    "metodoPago": "Yape",
    "montoFormateado": "50.00",
    "moneda": "PEN",
    "minutosExpiracion": 15,
    "fechaExpiracion": "2025-10-28T10:15:00Z",
    "informacionAdicional": "Escanea el código QR con tu app Yape y envía S/ 50.00 al número 901269594"
  }
}
```

**Frontend:**
```javascript
// Mostrar QR Code
const qrImage = `data:image/png;base64,${response.data.qrCodeBase64}`;
document.getElementById('qr-image').src = qrImage;

// Mostrar temporizador de expiración
const expiracion = new Date(response.data.fechaExpiracion);
startCountdown(expiracion);

// Guardar IDs para siguiente paso
const idPago = response.data.pago.idPago;
```

#### **PASO 2: Cliente paga con Yape**

*Cliente abre app Yape → Escanea QR → Paga → Obtiene código de operación*

Ejemplo: Código de operación Yape: **"ABC12345"**

#### **PASO 3: Confirmar Pago**

**Frontend muestra input para código:**
```
┌──────────────────────────────┐
│ Ingresa el código de         │
│ operación de Yape:           │
│                              │
│ [ABC12345_____________]      │
│                              │
│ [Confirmar Pago]             │
└──────────────────────────────┘
```

```javascript
// Request
POST /api/Pago/confirmar
Content-Type: application/json
Authorization: Bearer {token}

{
  "idPago": 456,
  "codigoOperacion": "ABC12345"
}
```

```javascript
// Response (200 OK)
{
  "isSuccess": true,
  "message": "Pago confirmado exitosamente. Código de operación: ABC12345",
  "data": {
    "idPago": 456,
    "idReserva": 123,
    "monto": 50.00,
    "montoAdelanto": 50.00,
    "montoPendiente": 0,
    "moneda": "PEN",
    "codigoOperacion": "ABC12345",
    "idMetodoPago": 4,
    "idEstadoPago": 1  // ✅ PAGADO
  }
}
```

**Frontend muestra éxito:**
```
┌──────────────────────────────┐
│ ✅ Reserva Confirmada        │
│                              │
│ Cancha: Cancha 1             │
│ Fecha: 28/10/2025            │
│ Hora: 10:00 - 12:00          │
│ Monto: S/ 50.00              │
│                              │
│ Código: ABC12345             │
│                              │
│ [Ver Mis Reservas]           │
└──────────────────────────────┘
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO** (código 02)
- ✅ Pago: **PAGADO** (código 01)

---

## FLUJO 2: RESERVA CON PLIN

### 🎯 Caso de Uso
Idéntico a Yape, solo cambia el método de pago.

### 📡 Implementación

#### **PASO 1: Crear Reserva**

```javascript
POST /api/Reserva

{
  "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
  "idCancha": 5,
  "idMetodoPago": 5,  // 5 = Plin
  "fecha": "2025-10-28T00:00:00",
  "horaInicio": "10:00:00",
  "horaFin": "12:00:00",
  "monto": 50.00
}
```

**Response:** Igual que Yape, pero con `"metodoPago": "Plin"`

#### **PASO 2 y 3:** Idénticos a Yape

**Código de operación Plin:** También 6-10 caracteres alfanuméricos (ej: "XYZ789")

---

## FLUJO 3: RESERVA CON EFECTIVO (SIN ADELANTO)

### 🎯 Caso de Uso
Operador registra reserva, cliente llega y paga todo en efectivo.

### 📱 Interfaz de Usuario (Panel Operador)

**Pantalla 1: Registrar reserva**
```
┌──────────────────────────────┐
│ Nueva Reserva - Efectivo     │
│                              │
│ Cliente: Juan Pérez          │
│ Cancha: Cancha 2             │
│ Fecha: 28/10/2025            │
│ Hora: 14:00 - 16:00          │
│ Monto: S/ 80.00              │
│                              │
│ [Registrar Reserva]          │
└──────────────────────────────┘
```

**Pantalla 2: Cliente llega y paga**
```
┌──────────────────────────────┐
│ Confirmar Pago en Efectivo   │
│                              │
│ Reserva #123                 │
│ Cliente: Juan Pérez          │
│ Monto: S/ 80.00              │
│                              │
│ Recibo N°: [REC-001___]      │
│                              │
│ [Confirmar Pago Recibido]    │
└──────────────────────────────┘
```

### 📡 Implementación

#### **PASO 1: Operador crea reserva**

```javascript
POST /api/Reserva

{
  "idUsuario": "789e4567-e89b-12d3-a456-426614174000",
  "idCancha": 2,
  "idMetodoPago": 2,  // 2 = Efectivo
  "fecha": "2025-10-28T00:00:00",
  "horaInicio": "14:00:00",
  "horaFin": "16:00:00",
  "monto": 80.00
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Efectivo.",
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
    "qrCodeBase64": null,  // No hay QR para efectivo
    "informacionAdicional": "Reserva registrada. El cliente debe pagar S/ 80.00 en efectivo..."
  }
}
```

**Frontend:**
```javascript
// Mostrar mensaje al operador
showAlert('Reserva registrada. Cliente debe pagar al llegar.');

// Guardar ID de pago
const idPago = response.data.pago.idPago;
```

#### **PASO 2: Cliente llega y paga completo**

```javascript
POST /api/Pago/confirmar

{
  "idPago": 457,
  "codigoOperacion": "EFECTIVO-REC-001"  // Número de recibo interno
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Pago confirmado exitosamente. Código de operación: EFECTIVO-REC-001",
  "data": {
    "idPago": 457,
    "monto": 80.00,
    "montoAdelanto": 80.00,
    "montoPendiente": 0,
    "codigoOperacion": "EFECTIVO-REC-001",
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
Cliente da adelanto del 50% o más al momento de reservar, la reserva se confirma automáticamente. Luego completa el pago al llegar.

### 📱 Interfaz de Usuario (Panel Operador)

**Pantalla 1: Registrar reserva con adelanto**
```
┌──────────────────────────────┐
│ Nueva Reserva - Efectivo     │
│                              │
│ Cliente: María López         │
│ Cancha: Cancha 3             │
│ Fecha: 28/10/2025            │
│ Hora: 16:00 - 18:00          │
│ Monto Total: S/ 100.00       │
│                              │
│ ☑ Cliente da adelanto        │
│ Adelanto: [50.00_______]     │
│                              │
│ [Registrar Reserva]          │
└──────────────────────────────┘
```

**Pantalla 2: Completar pago**
```
┌──────────────────────────────┐
│ Completar Pago               │
│                              │
│ Reserva #125                 │
│ Cliente: María López         │
│ Monto Total: S/ 100.00       │
│ Pagado: S/ 50.00 ✅          │
│ Pendiente: S/ 50.00          │
│                              │
│ Monto Restante: [50.00___]   │
│ Recibo N°: [REC-003___]      │
│                              │
│ [Completar Pago]             │
└──────────────────────────────┘
```

### 📡 Implementación

#### **PASO 1: Operador crea reserva CON adelanto**

```javascript
POST /api/Reserva

{
  "idUsuario": "abc-456-def",
  "idCancha": 3,
  "idMetodoPago": 2,  // Efectivo
  "fecha": "2025-10-28T00:00:00",
  "horaInicio": "16:00:00",
  "horaFin": "18:00:00",
  "monto": 100.00,
  "montoAdelanto": 50.00  // ← ADELANTO SE ENVÍA AQUÍ
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Efectivo.",
  "data": {
    "reserva": {
      "idReserva": 125,
      "idEstadoReserva": 2  // ✅ CONFIRMADO (porque adelanto >= 50%)
    },
    "pago": {
      "idPago": 458,
      "monto": 100.00,
      "montoAdelanto": 50.00,      // ← Adelanto registrado
      "montoPendiente": 50.00,      // ← Pendiente
      "idEstadoPago": 4  // 🟠 PARCIAL
    },
    "informacionAdicional": "Reserva confirmada. Adelanto recibido: S/ 50.00. Pendiente: S/ 50.00"
  }
}
```

**Frontend:**
```javascript
// Mostrar alerta
showSuccess('¡Reserva confirmada! Adelanto de 50% registrado.');

// Actualizar interfaz
updatePaymentStatus({
  total: 100,
  paid: 50,
  pending: 50,
  percentage: 50,
  status: 'PARCIAL',
  reservationStatus: 'CONFIRMADO'
});
```

**Estado actual:**
- ✅ Reserva: **CONFIRMADO** (porque 50% >= 50% mínimo)
- 🟠 Pago: **PARCIAL**

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
  "message": "Pago completado exitosamente. Total pagado: S/ 100.00. Reserva confirmada.",
  "data": {
    "idPago": 458,
    "monto": 100.00,
    "montoAdelanto": 100.00,     // 50 + 50
    "montoPendiente": 0,
    "numeroReferencia": "REC-003",
    "idEstadoPago": 1  // ✅ PAGADO
  }
}
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO**
- ✅ Pago: **PAGADO**

---

### 📊 Variante: Adelanto menor al 50%

Si el adelanto es **menor al 50%**, la reserva NO se confirma automáticamente:

```javascript
POST /api/Reserva

{
  "idUsuario": "abc-789-ghi",
  "idCancha": 4,
  "idMetodoPago": 2,
  "fecha": "2025-10-28T00:00:00",
  "horaInicio": "18:00:00",
  "horaFin": "20:00:00",
  "monto": 100.00,
  "montoAdelanto": 30.00  // Solo 30%
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Efectivo.",
  "data": {
    "reserva": {
      "idReserva": 126,
      "idEstadoReserva": 1  // ⏳ PENDIENTE (30% < 50%)
    },
    "pago": {
      "idPago": 459,
      "monto": 100.00,
      "montoAdelanto": 30.00,
      "montoPendiente": 70.00,
      "idEstadoPago": 4  // 🟠 PARCIAL
    },
    "informacionAdicional": "Adelanto registrado: S/ 30.00. Se requiere mínimo 50% para confirmar. Pendiente: S/ 70.00"
  }
}
```

**Estado:**
- ⏳ Reserva: **PENDIENTE** (porque 30% < 50%)
- 🟠 Pago: **PARCIAL**

**Frontend debe mostrar:**
```
⚠️ Adelanto recibido pero insuficiente
Se requiere mínimo 50% (S/ 50.00) para confirmar la reserva.
Actual: S/ 30.00 (30%)
```

**Nota:** La reserva se confirmará automáticamente cuando el cliente complete el pago restante usando `POST /api/Pago/completar-pago`.

---

## FLUJO 5: RESERVA CON TRANSFERENCIA (MERCADOPAGO)

### 🎯 Caso de Uso
**Estado actual:** 🚧 Placeholder (no implementado)

### 📡 Comportamiento Actual

```javascript
POST /api/Reserva

{
  "idMetodoPago": 3  // Transferencia
}
```

```javascript
// Response
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Transferencia.",
  "data": {
    "informacionAdicional": "⚠️ MÉTODO DE PAGO NO IMPLEMENTADO AÚN.\n\nSe requiere configurar la integración con MercadoPago u otro proveedor de pagos.\nMonto a pagar: S/ 50.00\n\nVer FLUJOS_PAGO.md para instrucciones de implementación."
  }
}
```

**Frontend debe mostrar:**
```
┌──────────────────────────────┐
│ ⚠️ Método No Disponible      │
│                              │
│ La transferencia bancaria    │
│ aún no está habilitada.      │
│                              │
│ Por favor elige otro método: │
│ • Yape                       │
│ • Plin                       │
│ • Efectivo                   │
│                              │
│ [Volver]                     │
└──────────────────────────────┘
```

---

## MANEJO DE ERRORES

### Errores Comunes y Respuestas

#### **Error 1: Cancha no disponible en ese horario**

```javascript
// Response (200 OK - isSuccess: false)
{
  "isSuccess": false,
  "message": "Ya existe una reserva para esta cancha en el horario seleccionado (10:00 - 12:00).",
  "data": null
}
```

**Frontend:**
```javascript
if (!response.isSuccess) {
  showError(response.message);
  // Sugerir otro horario
  suggestAlternativeTime();
}
```

#### **Error 2: Código de operación inválido**

```javascript
// Response
{
  "isSuccess": false,
  "message": "El formato del código de operación no es válido.",
  "data": null
}
```

**Frontend:**
```javascript
// Validar formato antes de enviar
function validateYapeCode(code) {
  // 6-10 caracteres alfanuméricos
  const regex = /^[A-Z0-9]{6,10}$/i;
  return regex.test(code);
}
```

#### **Error 3: Pago expirado**

```javascript
// Response
{
  "isSuccess": false,
  "message": "El tiempo para completar el pago ha expirado. Tiempo límite: 15 minutos.",
  "data": null
}
```

**Frontend:**
```javascript
// Mostrar temporizador y manejar expiración
function onExpiration() {
  showError('Tiempo expirado. Por favor crea una nueva reserva.');
  redirectTo('/nueva-reserva');
}
```

#### **Error 4: Adelanto excede monto pendiente**

```javascript
// Response
{
  "isSuccess": false,
  "message": "El adelanto excede el monto total. Monto total: S/ 100.00, Ya pagado: S/ 60.00, Pendiente: S/ 40.00",
  "data": null
}
```

**Frontend:**
```javascript
// Validar monto antes de enviar
if (adelanto > montoPendiente) {
  showError(`Monto máximo permitido: S/ ${montoPendiente.toFixed(2)}`);
  return;
}
```

---

## EJEMPLOS DE INTERFAZ

### 🎨 Componente: Card de Método de Pago

```jsx
// React Example
function PaymentMethodCard({ method, selected, onSelect }) {
  const icons = {
    'Yape': '📱',
    'Plin': '💳',
    'Efectivo': '💵'
  };

  return (
    <div
      className={`payment-card ${selected ? 'selected' : ''}`}
      onClick={() => onSelect(method)}
    >
      <div className="icon">{icons[method.nombre]}</div>
      <h3>{method.nombre}</h3>
      {method.codigo === '04' && <span className="badge">Instantáneo</span>}
      {method.codigo === '02' && <span className="badge">En persona</span>}
    </div>
  );
}
```

### 🎨 Componente: QR Code Display

```jsx
function QRCodeDisplay({ qrBase64, monto, metodoPago, expiresAt }) {
  const [timeLeft, setTimeLeft] = useState(null);

  useEffect(() => {
    const timer = setInterval(() => {
      const now = new Date();
      const expiry = new Date(expiresAt);
      const diff = expiry - now;

      if (diff <= 0) {
        clearInterval(timer);
        onExpired();
      } else {
        setTimeLeft(formatTime(diff));
      }
    }, 1000);

    return () => clearInterval(timer);
  }, [expiresAt]);

  return (
    <div className="qr-container">
      <h2>Escanea con {metodoPago}</h2>
      <img src={`data:image/png;base64,${qrBase64}`} alt="QR Code" />
      <p className="amount">S/ {monto.toFixed(2)}</p>
      <p className="timer">Expira en: {timeLeft}</p>
      <button onClick={onConfirm}>Ya pagué - Confirmar</button>
    </div>
  );
}
```

### 🎨 Componente: Payment Progress (Adelantos)

```jsx
function PaymentProgress({ total, paid, pending }) {
  const percentage = (paid / total) * 100;

  return (
    <div className="payment-progress">
      <div className="progress-bar">
        <div
          className="progress-fill"
          style={{ width: `${percentage}%` }}
        />
      </div>

      <div className="amounts">
        <div>
          <span className="label">Total</span>
          <span className="value">S/ {total.toFixed(2)}</span>
        </div>
        <div className="paid">
          <span className="label">Pagado</span>
          <span className="value">S/ {paid.toFixed(2)}</span>
        </div>
        <div className="pending">
          <span className="label">Pendiente</span>
          <span className="value">S/ {pending.toFixed(2)}</span>
        </div>
      </div>

      <p className="percentage">{percentage.toFixed(0)}% completado</p>

      {percentage >= 50 && pending > 0 && (
        <div className="badge success">
          ✅ Reserva confirmada con adelanto
        </div>
      )}
    </div>
  );
}
```

---

## 📋 CHECKLIST DE IMPLEMENTACIÓN FRONTEND

### Página: Nueva Reserva

- [ ] Formulario para seleccionar cancha, fecha, hora
- [ ] Selector de método de pago (Yape, Plin, Efectivo)
- [ ] Validación de disponibilidad en tiempo real
- [ ] Cálculo automático de monto según horas reservadas
- [ ] Botón "Crear Reserva"

### Página: Pago con Yape/Plin

- [ ] Mostrar QR Code en Base64
- [ ] Temporizador de expiración (15 minutos)
- [ ] Input para código de operación
- [ ] Validación de formato (6-10 alfanuméricos)
- [ ] Botón "Confirmar Pago"
- [ ] Redirección a "Reserva Confirmada" al éxito

### Página: Pago con Efectivo (Panel Operador)

- [ ] Checkbox "Cliente da adelanto" al crear reserva
- [ ] Input para monto de adelanto (opcional, solo si checkbox activo)
- [ ] Validación: adelanto no puede exceder el monto total
- [ ] Mensaje si adelanto < 50%: "Reserva quedará PENDIENTE"
- [ ] Mensaje si adelanto >= 50%: "Reserva se confirmará automáticamente"
- [ ] Buscar reserva existente por ID o cliente
- [ ] Mostrar detalles de pago (total, pagado, pendiente)
- [ ] Progress bar mostrando porcentaje pagado
- [ ] Badge si adelanto >= 50% (Reserva Confirmada)
- [ ] Botón "Completar Pago" cuando pago está en estado PARCIAL
- [ ] Input para número de recibo

### Página: Mis Reservas

- [ ] Lista de reservas con filtros por estado
- [ ] Badges de estado (Pendiente, Confirmado, Pagado)
- [ ] Ver detalles de pago
- [ ] Opción de cancelar (solo si pendiente)

---

## 🔍 CONSULTAS ÚTILES

### Obtener detalles de una reserva

```javascript
GET /api/Reserva/{idReserva}

// Response incluye estado de pago
{
  "data": {
    "idReserva": 123,
    "fecha": "2025-10-28",
    "horaInicio": "10:00",
    "horaFin": "12:00",
    "monto": 50.00,
    "idEstadoReserva": 2,  // CONFIRMADO
    "pago": {
      "idPago": 456,
      "idEstadoPago": 1,  // PAGADO
      "montoAdelanto": 50.00,
      "montoPendiente": 0
    }
  }
}
```

### Buscar reservas por usuario

```javascript
POST /api/Reserva/search

{
  "filter": {
    "idUsuario": "123e4567-e89b-12d3-a456-426614174000"
  },
  "pageNumber": 1,
  "pageSize": 10
}
```

### Consultar estado de pago

```javascript
GET /api/Pago/{idPago}

{
  "data": {
    "idPago": 456,
    "monto": 100.00,
    "montoAdelanto": 50.00,
    "montoPendiente": 50.00,
    "idEstadoPago": 4,  // PARCIAL
    "codigoOperacion": null,
    "numeroReferencia": "REC-001"
  }
}
```

---

## 📞 SOPORTE

**Documentación adicional:**
- `FLUJOS_PAGO.md` - Flujos detallados del backend
- `GUIA_USO_ADELANTOS.md` - Sistema de adelantos
- `MIGRACIONES_BD.sql` - Estructura de base de datos

**Errores o dudas:** Ver sección "Manejo de Errores" arriba

---

**Versión:** 1.0
**Última actualización:** 2025-10-27
**Estado:** ✅ Producción (Yape, Plin, Efectivo) | 🚧 Desarrollo (Transferencia/MercadoPago)
