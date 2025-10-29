# 📘 GUÍA DE USO - SISTEMA DE ADELANTOS

## ✅ IMPLEMENTACIÓN COMPLETADA

El sistema de adelantos para pagos en **EFECTIVO** está completamente implementado y listo para usar.

---

## 🎯 FLUJO SIMPLIFICADO DE ADELANTOS

### **Escenario:** Operador reserva una cancha para un cliente que pagará en efectivo

---

## 📌 FLUJO 1: Reserva CON adelanto (50% o más)

### **PASO 1: Crear Reserva con Adelanto**

**Endpoint:** `POST /api/Reserva`

```json
{
  "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
  "idCancha": 1,
  "idMetodoPago": 2,  // 2 = Efectivo (ver Constants.METODO_PAGO)
  "fecha": "2025-10-28",
  "horaInicio": "10:00",
  "horaFin": "12:00",
  "monto": 100.00,
  "montoAdelanto": 50.00  // ← NUEVO: Adelanto se envía directamente aquí
}
```

**Respuesta:**
```json
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Efectivo.",
  "data": {
    "reserva": {
      "idReserva": 123,
      "idEstadoReserva": 2,  // CONFIRMADO (porque adelanto >= 50%)
      ...
    },
    "pago": {
      "idPago": 456,
      "monto": 100.00,
      "montoAdelanto": 50.00,
      "montoPendiente": 50.00,
      "idEstadoPago": 4,  // PARCIAL
      ...
    },
    "informacionAdicional": "Reserva confirmada. Adelanto recibido: S/ 50.00. Pendiente: S/ 50.00"
  }
}
```

**Estado actual:**
- ✅ Reserva creada: **CONFIRMADO** ✅ (porque adelanto >= 50%)
- ✅ Pago creado: **PARCIAL** 🟡
- ✅ Monto total: S/ 100.00
- ✅ Adelanto: S/ 50.00
- ✅ Pendiente: S/ 50.00

**Reglas aplicadas:**
- ✅ Adelanto >= 50% → Reserva se CONFIRMA automáticamente
- ✅ Estado de pago cambia a PARCIAL
- ✅ MontoAdelanto solo válido para EFECTIVO

---

### **PASO 2: Cliente completa el pago restante**

**Endpoint:** `POST /api/Pago/completar-pago`

```json
{
  "idPago": 456,
  "montoRestante": 50.00,
  "numeroRecibo": "REC-002"  // Opcional
}
```

**Respuesta:**
```json
{
  "isSuccess": true,
  "message": "Pago completado exitosamente. Total pagado: S/ 100.00. Reserva confirmada.",
  "data": {
    "idPago": 456,
    "monto": 100.00,
    "montoAdelanto": 100.00,  // 50 + 50
    "montoPendiente": 0,
    "idEstadoPago": 1,  // PAGADO
    "numeroReferencia": "REC-002"
  }
}
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO** ✅
- ✅ Pago: **PAGADO** ✅
- ✅ Total pagado: S/ 100.00

---

## 📌 FLUJO 2: Reserva SIN adelanto

### **PASO 1: Crear Reserva sin Adelanto**

**Endpoint:** `POST /api/Reserva`

```json
{
  "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
  "idCancha": 1,
  "idMetodoPago": 2,  // 2 = Efectivo
  "fecha": "2025-10-28",
  "horaInicio": "10:00",
  "horaFin": "12:00",
  "monto": 100.00
  // montoAdelanto: null o 0 (no se envía)
}
```

**Respuesta:**
```json
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Efectivo.",
  "data": {
    "reserva": {
      "idReserva": 123,
      "idEstadoReserva": 1,  // PENDIENTE (sin adelanto)
      ...
    },
    "pago": {
      "idPago": 456,
      "monto": 100.00,
      "montoAdelanto": 0,
      "montoPendiente": 100.00,
      "idEstadoPago": 2,  // PENDIENTE
      ...
    }
  }
}
```

---

### **PASO 2: Cliente paga completo al llegar**

**Endpoint:** `POST /api/Pago/confirmar`

```json
{
  "idPago": 456,
  "codigoOperacion": "RECIBO-001"  // Número de recibo interno
}
```

**Respuesta:**
```json
{
  "isSuccess": true,
  "message": "Pago confirmado exitosamente.",
  "data": {
    "idPago": 456,
    "monto": 100.00,
    "montoAdelanto": 0,
    "montoPendiente": 0,
    "idEstadoPago": 1  // PAGADO
  }
}
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO** ✅
- ✅ Pago: **PAGADO** ✅

---

## 📌 FLUJO 3: Reserva con adelanto del 100%

### **PASO 1: Crear Reserva con Pago Completo**

**Endpoint:** `POST /api/Reserva`

```json
{
  "idUsuario": "123e4567-e89b-12d3-a456-426614174000",
  "idCancha": 1,
  "idMetodoPago": 2,
  "fecha": "2025-10-28",
  "horaInicio": "10:00",
  "horaFin": "12:00",
  "monto": 100.00,
  "montoAdelanto": 100.00  // Pago completo desde el inicio
}
```

**Respuesta:**
```json
{
  "isSuccess": true,
  "message": "Reserva creada exitosamente con método de pago: Efectivo.",
  "data": {
    "reserva": {
      "idReserva": 123,
      "idEstadoReserva": 2,  // CONFIRMADO
      ...
    },
    "pago": {
      "idPago": 456,
      "monto": 100.00,
      "montoAdelanto": 100.00,
      "montoPendiente": 0,
      "idEstadoPago": 1,  // PAGADO (ya completado)
      ...
    }
  }
}
```

**Estado final:**
- ✅ Reserva: **CONFIRMADO** ✅
- ✅ Pago: **PAGADO** ✅ (sin necesidad de completar)

---

## 🔧 CONFIGURACIÓN REQUERIDA

### **appsettings.json**

```json
{
  "Pago": {
    "MinutosExpiracion": 15,
    "TelefonoYape": "901269594",
    "TelefonoPlin": "901269594",
    "PorcentajeMinimoAdelanto": 50  // ← NUEVO: Porcentaje mínimo para confirmar reserva
  }
}
```

---

## 📊 ESTADOS Y TRANSICIONES

### **Estados de Pago**

| Código | Estado     | Cuándo                          |
|--------|------------|---------------------------------|
| 01     | Pagado     | Monto completamente pagado      |
| 02     | Pendiente  | Sin pagar                       |
| 03     | Rechazado  | Pago fallido                    |
| 04     | Parcial    | Con adelanto (solo efectivo)    |

### **Estados de Reserva**

| Código | Estado      | Cuándo                           |
|--------|-------------|----------------------------------|
| 01     | Pendiente   | Esperando pago                   |
| 02     | Confirmado  | Pago >= 50% o completado         |
| 03     | Cancelado   | Expiró o rechazado               |

### **Diagrama de Transiciones (Efectivo con Adelanto)**

```
Reserva PENDIENTE + Pago PENDIENTE
           ↓
    [Adelanto >= 50%]
           ↓
Reserva CONFIRMADO + Pago PARCIAL
           ↓
   [Completa pago restante]
           ↓
Reserva CONFIRMADO + Pago PAGADO ✅
```

---

## ✅ VALIDACIONES IMPLEMENTADAS

### **CreateReserva (con MontoAdelanto)**
- ✅ MontoAdelanto solo válido para método de pago **EFECTIVO**
- ✅ MontoAdelanto debe ser mayor a 0 (si se envía)
- ✅ MontoAdelanto no puede exceder el Monto total
- ✅ Si adelanto >= 50%: Reserva se CONFIRMA automáticamente
- ✅ Si adelanto < 50%: Reserva queda PENDIENTE
- ✅ Si adelanto = 100%: Estado de pago cambia a PAGADO directamente

### **CompletarPago**
- ✅ Solo para método de pago **EFECTIVO**
- ✅ Pago debe estar en estado **PARCIAL**
- ✅ Monto restante debe coincidir exactamente con MontoPendiente
- ✅ Al completar: Estado cambia a PAGADO
- ✅ Reserva siempre se CONFIRMA

---

## 🚨 ERRORES COMUNES

### **Error: "Solo se permiten adelantos para pagos en efectivo"**
**Causa:** Intentaste enviar MontoAdelanto con método de pago diferente a EFECTIVO
**Solución:** Adelantos solo funcionan con método de pago **EFECTIVO** (código 02). Para Yape/Plin no envíes MontoAdelanto.

### **Error: "El monto del adelanto no puede ser mayor que el monto total"**
**Causa:** MontoAdelanto > Monto total
**Solución:** Asegúrate de que MontoAdelanto <= Monto

### **Error: "El pago debe estar en estado Parcial para completarlo"**
**Causa:** Intentaste completar un pago que no tiene adelanto
**Solución:** Si el pago está PENDIENTE, usa `POST /api/Pago/confirmar` en vez de completar-pago

### **Error: "El monto restante no coincide"**
**Causa:** El monto enviado no es exactamente igual al MontoPendiente
**Solución:** Consulta el pago primero (`GET /api/Pago/{idPago}`) para ver cuánto falta exactamente

---

## 📝 EJEMPLOS DE USO

### **Ejemplo 1: Adelanto del 60% (confirma reserva automáticamente)**

```bash
# 1. Crear reserva con adelanto de S/ 60 (60% de S/ 100)
POST /api/Reserva
{
  "idUsuario": "...",
  "idCancha": 1,
  "idMetodoPago": 2,  // Efectivo
  "fecha": "2025-10-28",
  "horaInicio": "10:00",
  "horaFin": "12:00",
  "monto": 100.00,
  "montoAdelanto": 60.00
}
# Resultado: Reserva CONFIRMADO ✅ (60% >= 50%)
#            Pago PARCIAL 🟡
#            Pendiente: S/ 40.00

# 2. Cliente llega y completa con S/ 40
POST /api/Pago/completar-pago
{
  "idPago": 456,
  "montoRestante": 40.00
}
# Resultado: Pago PAGADO ✅
```

### **Ejemplo 2: Adelanto del 30% (NO confirma reserva)**

```bash
# 1. Crear reserva con adelanto de S/ 30 (30% de S/ 100)
POST /api/Reserva
{
  "idUsuario": "...",
  "idCancha": 1,
  "idMetodoPago": 2,
  "fecha": "2025-10-28",
  "horaInicio": "10:00",
  "horaFin": "12:00",
  "monto": 100.00,
  "montoAdelanto": 30.00
}
# Resultado: Reserva PENDIENTE ⏳ (30% < 50%)
#            Pago PARCIAL 🟡
#            Pendiente: S/ 70.00

# 2. Cliente completa el pago restante S/ 70
POST /api/Pago/completar-pago
{
  "idPago": 456,
  "montoRestante": 70.00
}
# Resultado: Reserva CONFIRMADO ✅
#            Pago PAGADO ✅
```

### **Ejemplo 3: Sin adelanto (pago completo al llegar)**

```bash
# 1. Crear reserva sin adelanto
POST /api/Reserva
{
  "idUsuario": "...",
  "idCancha": 1,
  "idMetodoPago": 2,
  "fecha": "2025-10-28",
  "horaInicio": "10:00",
  "horaFin": "12:00",
  "monto": 100.00
  // No se envía montoAdelanto
}
# Resultado: Reserva PENDIENTE ⏳
#            Pago PENDIENTE ⏳

# 2. Cliente llega y paga todo
POST /api/Pago/confirmar
{
  "idPago": 456,
  "codigoOperacion": "RECIBO-001"
}
# Resultado: Reserva CONFIRMADO ✅
#            Pago PAGADO ✅
```

---

## 🔍 CONSULTAR ESTADO DE PAGO

Para ver el estado actual de un pago:

```bash
GET /api/Pago/{idPago}
```

**Respuesta:**
```json
{
  "idPago": 456,
  "monto": 100.00,
  "montoAdelanto": 50.00,
  "montoPendiente": 50.00,
  "idEstadoPago": 4,  // 4 = PARCIAL
  "numeroReferencia": "REC-001"
}
```

---

## ✨ RESUMEN DE ENDPOINTS

| Endpoint | Método | Uso |
|----------|--------|-----|
| `/api/Reserva` | POST | Crear reserva con adelanto opcional (campo `montoAdelanto`) |
| `/api/Pago/confirmar` | POST | Confirmar pago completo (Yape/Plin/Efectivo sin adelanto) |
| `/api/Pago/completar-pago` | POST | Completar pago parcial (solo Efectivo con adelanto) |
| `/api/Pago/{idPago}` | GET | Consultar estado de pago |

---

## 🎯 FLUJO SIMPLIFICADO RESUMIDO

```
┌─────────────────────────────────────────────────────────┐
│  CREAR RESERVA CON EFECTIVO                            │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  POST /api/Reserva                                      │
│  {                                                      │
│    "monto": 100.00,                                     │
│    "montoAdelanto": 50.00  ← OPCIONAL                   │
│  }                                                      │
│                                                         │
│  ┌──────────────┬──────────────┬──────────────┐         │
│  │ Sin adelanto │ Con 30%      │ Con 50%+     │         │
│  ├──────────────┼──────────────┼──────────────┤         │
│  │ Reserva:     │ Reserva:     │ Reserva:     │         │
│  │ PENDIENTE    │ PENDIENTE    │ CONFIRMADO✅  │         │
│  │              │              │              │         │
│  │ Pago:        │ Pago:        │ Pago:        │         │
│  │ PENDIENTE    │ PARCIAL      │ PARCIAL      │         │
│  └──────────────┴──────────────┴──────────────┘         │
│                                                         │
│  LUEGO:                                                 │
│  • Sin adelanto → POST /api/Pago/confirmar              │
│  • Con adelanto → POST /api/Pago/completar-pago         │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

**¡Sistema de adelantos listo para usar!** 🚀
