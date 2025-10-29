# 📋 FLUJOS DE PAGO - SISTEMA DE RESERVAS

## 🎯 MÉTODOS DE PAGO IMPLEMENTADOS

### 1. YAPE (Código: 04)
**Flujo:**
```
Cliente → Crea Reserva
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE
└─> Recibe: QR Code (15 min expiración)

Cliente → Escanea QR y paga
└─> Obtiene código: "ABC123"

Cliente → Confirma pago
POST /api/Pago/confirmar
{
  "idPago": 123,
  "codigoOperacion": "ABC123"
}
├─> Validación: 6-10 caracteres alfanuméricos
├─> Pago: PAGADO ✅
└─> Reserva: CONFIRMADO ✅
```

---

### 2. PLIN (Código: 05)
**Flujo:** Igual que Yape
```
Cliente → Crea Reserva
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE
└─> Recibe: QR Code (15 min expiración)

Cliente → Escanea QR y paga
└─> Obtiene código: "XYZ789"

Cliente → Confirma pago
POST /api/Pago/confirmar
{
  "idPago": 123,
  "codigoOperacion": "XYZ789"
}
├─> Validación: 6-10 caracteres alfanuméricos
├─> Pago: PAGADO ✅
└─> Reserva: CONFIRMADO ✅
```

---

### 3. EFECTIVO (Código: 02) - CON ADELANTOS

#### 3.1 Flujo SIN adelanto
```
Operador → Crea Reserva (manual)
POST /api/Reserva
{
  "idUsuario": "...",
  "idCancha": 1,
  "idMetodoPago": 2,  // Efectivo
  "monto": 100.00,
  ...
}
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE (Monto: 100)
└─> Info: "Cliente debe pagar S/ 100.00 en efectivo"

Cliente → Llega y paga S/ 100 completo
│
Operador → Confirma pago recibido
POST /api/Pago/confirmar
{
  "idPago": 123,
  "codigoOperacion": "RECIBO-001" // Número de recibo interno
}
├─> Pago: PAGADO ✅
└─> Reserva: CONFIRMADO ✅
```

#### 3.2 Flujo CON adelanto (FUTURO - requiere migración BD)
```
⚠️ REQUIERE MIGRACIÓN:
ALTER TABLE Pago ADD MontoAdelanto DECIMAL(18,2) DEFAULT 0;
ALTER TABLE Pago ADD MontoPendiente DECIMAL(18,2) DEFAULT 0;

Operador → Crea Reserva
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE
│   ├─> Monto: 100
│   ├─> MontoAdelanto: 0
│   └─> MontoPendiente: 100
└─> Info: "Cliente debe pagar S/ 100.00"

Cliente → Da adelanto de S/ 30
│
Operador → Registra adelanto
POST /api/Pago/registrar-adelanto
{
  "idPago": 123,
  "montoAdelanto": 30.00
}
├─> Pago: PARCIAL 🟡
│   ├─> Monto: 100
│   ├─> MontoAdelanto: 30
│   └─> MontoPendiente: 70
└─> Reserva: CONFIRMADO ✅ (si adelanto >= 50% del total)

Cliente → Llega y paga restante S/ 70
│
Operador → Completa pago
POST /api/Pago/completar-pago
{
  "idPago": 123,
  "montoRestante": 70.00
}
├─> Pago: PAGADO ✅
│   ├─> Monto: 100
│   ├─> MontoAdelanto: 100 (30 + 70)
│   └─> MontoPendiente: 0
└─> Reserva: CONFIRMADO ✅
```

**Reglas de negocio para adelantos:**
- ✅ Adelanto mínimo: 50% del monto total
- ✅ Con adelanto >= 50%: Reserva se confirma
- ❌ Con adelanto < 50%: Reserva sigue PENDIENTE
- ⏰ Si no completa pago antes de la fecha: Reserva se CANCELA

---

### 4. TRANSFERENCIA (Código: 03) - INTEGRACIÓN EXTERNA

**Estado actual:** Preparado para MercadoPago u otro proveedor

```
🚧 IMPLEMENTACIÓN FUTURA - PLACEHOLDER
```

#### 4.1 Flujo planeado con MercadoPago:
```
Cliente → Crea Reserva
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE
└─> Llamada a API MercadoPago
    └─> Recibe: URL de pago (preference_id)

Cliente → Redirigido a MercadoPago
└─> Completa pago en plataforma externa

MercadoPago → Webhook notifica resultado
POST /api/Pago/webhook-mercadopago
{
  "payment_id": "123456789",
  "status": "approved",
  "external_reference": "pago-123"
}
├─> Pago: PAGADO ✅
└─> Reserva: CONFIRMADO ✅
```

**Pendiente implementar:**
- [ ] SDK de MercadoPago
- [ ] Endpoint para crear preference
- [ ] Webhook para recibir notificaciones
- [ ] Manejo de estados: pending, approved, rejected
- [ ] Configuración: Access Token, Public Key

**Configuración necesaria (appsettings.json):**
```json
{
  "MercadoPago": {
    "AccessToken": "APP_USR-xxxxx",
    "PublicKey": "APP_USR-xxxxx",
    "WebhookSecret": "xxxxx",
    "SuccessUrl": "https://tuapp.com/pago-exitoso",
    "FailureUrl": "https://tuapp.com/pago-fallido"
  }
}
```

---

## 📊 ESTADOS DE PAGO

| Código | Estado     | Descripción                                    |
|--------|------------|------------------------------------------------|
| 01     | Pagado     | Pago completado totalmente                     |
| 02     | Pendiente  | Sin pagar                                      |
| 03     | Rechazado  | Pago rechazado o fallido                       |
| 04     | Parcial    | Adelanto recibido (solo efectivo)              |

---

## 📊 ESTADOS DE RESERVA

| Código | Estado      | Cuándo se aplica                              |
|--------|-------------|-----------------------------------------------|
| 01     | Pendiente   | Esperando pago                                |
| 02     | Confirmado  | Pago completado o adelanto >= 50%             |
| 03     | Cancelado   | Expiró tiempo o pago rechazado                |

---

## 🔧 CONFIGURACIÓN ACTUAL

**appsettings.json:**
```json
{
  "Pago": {
    "MinutosExpiracion": 15,
    "TelefonoYape": "901269594",
    "TelefonoPlin": "901269594",
    "Transferencia": {
      "NumeroCuenta": "PENDIENTE",
      "CCI": "PENDIENTE",
      "NombreBanco": "Banco BCP",
      "TitularCuenta": "PENDIENTE"
    }
  }
}
```

---

## ⚙️ PRÓXIMOS PASOS

### Migración de BD para adelantos:
```sql
-- 1. Agregar estado PARCIAL a tabla EstadoPago
INSERT INTO EstadoPago (Codigo, Nombre, Activo)
VALUES ('04', 'Parcial', 1);

-- 2. Agregar campos para adelantos
ALTER TABLE Pago ADD MontoAdelanto DECIMAL(18,2) DEFAULT 0;
ALTER TABLE Pago ADD MontoPendiente DECIMAL(18,2) DEFAULT 0;

-- 3. Actualizar registros existentes
UPDATE Pago SET MontoPendiente = Monto WHERE IdEstadoPago IN (
  SELECT IdEstadoPago FROM EstadoPago WHERE Codigo = '02' -- Pendiente
);
```

### Endpoints a crear:
- [ ] `POST /api/Pago/registrar-adelanto` - Para efectivo
- [ ] `POST /api/Pago/completar-pago` - Para efectivo
- [ ] `POST /api/Pago/webhook-mercadopago` - Para transferencia

---

**Última actualización:** 2025-10-27
