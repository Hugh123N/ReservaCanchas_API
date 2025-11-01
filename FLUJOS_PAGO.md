# 📋 FLUJOS DE PAGO - SISTEMA DE RESERVAS CON IZIPAY

## 🎯 MÉTODOS DE PAGO IMPLEMENTADOS

### 1. YAPE (Código: 04) - VÍA IZIPAY ✅

**Flujo:**
```
Cliente → Crea Reserva
├─> Backend llama a Izipay API
├─> Izipay genera QR OFICIAL de Yape
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE
└─> Recibe: FormToken + PaymentURL + TransactionId

Cliente → Escanea QR oficial de Yape
├─> App Yape abre automáticamente con monto pre-cargado
├─> Cliente confirma pago en la app Yape
└─> Yape notifica a Izipay: PAGO EXITOSO

Izipay → Envía Webhook a tu backend
POST /api/IzipayWebhook/notification
{
  "transactionId": "abc123",
  "status": "PAID",
  "amount": 5000,
  "operationNumber": "YPE-789456"
}
├─> Backend valida firma HMAC-SHA256
├─> Busca Pago por TransactionId
├─> Actualiza: Pago: PAGADO ✅
└─> Actualiza: Reserva: CONFIRMADA ✅
```

**Ventajas sobre el flujo manual anterior:**
- ✅ QR **OFICIAL** de Yape (abre automáticamente la app)
- ✅ Verificación **AUTOMÁTICA** del pago (sin riesgo de fraude)
- ✅ Confirmación en **tiempo real** vía webhook
- ✅ No requiere operador para confirmar manualmente

---

### 2. PLIN (Código: 05) - VÍA IZIPAY ✅

**Flujo:** Idéntico a Yape
```
Cliente → Crea Reserva
├─> Backend llama a Izipay API
├─> Izipay genera QR OFICIAL de Plin
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE
└─> Recibe: FormToken + PaymentURL + TransactionId

Cliente → Escanea QR oficial de Plin
├─> App Plin abre automáticamente con monto pre-cargado
├─> Cliente confirma pago en la app Plin
└─> Plin notifica a Izipay: PAGO EXITOSO

Izipay → Envía Webhook a tu backend
POST /api/IzipayWebhook/notification
{
  "transactionId": "xyz789",
  "status": "PAID",
  "amount": 5000,
  "operationNumber": "PLN-123456"
}
├─> Backend valida firma HMAC-SHA256
├─> Actualiza: Pago: PAGADO ✅
└─> Actualiza: Reserva: CONFIRMADA ✅
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
  "codigoMetodoPago": "02",  // Efectivo
  "monto": 100.00
  // NO enviar montoAdelanto
}
├─> Reserva: PENDIENTE
├─> Pago: PENDIENTE (Monto: 100, Adelanto: 0, Pendiente: 100)
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

#### 3.2 Flujo CON adelanto >= 50%
```
Operador → Crea Reserva con adelanto
POST /api/Reserva
{
  "idUsuario": "...",
  "idCancha": 1,
  "codigoMetodoPago": "02",
  "monto": 100.00,
  "montoAdelanto": 50.00  // ← 50% o más
}
├─> Pago: PARCIAL 🟠
│   ├─> Monto: 100
│   ├─> MontoAdelanto: 50
│   └─> MontoPendiente: 50
└─> Reserva: CONFIRMADA ✅ (porque 50% >= 50% mínimo)

Cliente → Llega y paga restante S/ 50
│
Operador → Completa pago
POST /api/Pago/completar-pago
{
  "idPago": 123,
  "montoRestante": 50.00,
  "numeroRecibo": "REC-002"
}
├─> Pago: PAGADO ✅
│   ├─> MontoAdelanto: 100 (50 + 50)
│   └─> MontoPendiente: 0
└─> Reserva: CONFIRMADA ✅
```

#### 3.3 Flujo CON adelanto < 50%
```
Operador → Crea Reserva con adelanto insuficiente
POST /api/Reserva
{
  "idUsuario": "...",
  "idCancha": 1,
  "codigoMetodoPago": "02",
  "monto": 100.00,
  "montoAdelanto": 30.00  // ← Solo 30%
}
├─> Pago: PARCIAL 🟠
├─> Reserva: PENDIENTE ⏳ (porque 30% < 50%)
└─> Info: "Adelanto insuficiente. Requiere mínimo 50%"

Operador → Cliente completa hasta el 50% o más
POST /api/Pago/completar-pago
{
  "idPago": 123,
  "montoRestante": 20.00  // Ahora suma 50%
}
├─> Pago: PARCIAL 🟠 (50%)
└─> Reserva: CONFIRMADA ✅ (se confirma automáticamente)
```

**Reglas de negocio para adelantos:**
- ✅ Adelanto mínimo: **50%** del monto total
- ✅ Con adelanto >= 50%: Reserva se **CONFIRMA**
- ❌ Con adelanto < 50%: Reserva sigue **PENDIENTE**
- ⏰ Si no completa pago antes de la fecha: Reserva se **CANCELA**

---

### 4. TRANSFERENCIA (Código: 03) - INTEGRACIÓN FUTURA

**Estado actual:** 🚧 Placeholder (no implementado)

Puedes usar Izipay también para transferencias bancarias en el futuro.

---

## 📊 ESTADOS DE PAGO

| Código | Estado     | Descripción                                    |
|--------|------------|------------------------------------------------|
| 01     | Pagado     | Pago completado totalmente                     |
| 02     | Pendiente  | Sin pagar                                      |
| 03     | Rechazado  | Pago rechazado o fallido (por Izipay)          |
| 04     | Parcial    | Adelanto recibido (solo efectivo)              |

---

## 📊 ESTADOS DE RESERVA

| Código | Estado      | Cuándo se aplica                              |
|--------|-------------|-----------------------------------------------|
| 01     | Pendiente   | Esperando pago o adelanto < 50%               |
| 02     | Confirmado  | Pago completado o adelanto >= 50%             |
| 03     | Cancelado   | Pago rechazado o expiró tiempo                |

---

## 🔧 CONFIGURACIÓN ACTUAL

**appsettings.json:**
```json
{
  "Pago": {
    "MinutosExpiracion": 15,
    "PorcentajeMinimoAdelanto": 50
  },
  "Izipay": {
    "ApiUrl": "https://api.micuentaweb.pe/api-payment/V4/Charge",
    "Username": "TU_USERNAME_IZIPAY",
    "Password": "TU_PASSWORD_IZIPAY",
    "HmacSha256Key": "TU_HMAC_SHA256_KEY",
    "PublicKey": "TU_PUBLIC_KEY_IZIPAY",
    "PaymentPageUrl": "https://secure.micuentaweb.pe/payment",
    "SuccessUrl": "https://tuapp.com/pago-exitoso",
    "FailureUrl": "https://tuapp.com/pago-fallido",
    "WebhookUrl": "https://tuapi.com/api/IzipayWebhook/notification"
  }
}
```

---

## 🔐 SEGURIDAD DEL WEBHOOK

El webhook de Izipay incluye una **firma HMAC-SHA256** que debes validar:

```csharp
// Izipay envía la firma en el header X-Signature
var signature = Request.Headers["X-Signature"];

// Validar con tu HmacSha256Key
bool isValid = izipayService.ValidateWebhookSignature(requestBody, signature);

if (!isValid) {
    return Unauthorized(); // Rechazar webhook falso
}
```

Esto garantiza que **solo Izipay** puede confirmar pagos en tu sistema.

---

## 📋 CAMPOS AGREGADOS EN BASE DE DATOS

**Tabla: Pago**
```sql
ALTER TABLE Pago ADD IzipayTransactionId NVARCHAR(100) NULL;
ALTER TABLE Pago ADD IzipayFormToken NVARCHAR(500) NULL;
ALTER TABLE Pago ADD IzipayPaymentUrl NVARCHAR(500) NULL;

CREATE INDEX IX_Pago_IzipayTransactionId
ON Pago (IzipayTransactionId)
WHERE IzipayTransactionId IS NOT NULL;
```

---

## ⚙️ PRÓXIMOS PASOS

### Para poner en producción:

1. **Obtener credenciales de Izipay:**
   - Regístrate en https://www.izipay.pe/
   - Accede al BackOffice
   - Copia: Username, Password, HmacSha256Key, PublicKey

2. **Configurar Webhook en Izipay:**
   - Ve a: BackOffice → Configuración → Webhooks/IPN
   - URL: `https://tuapi.com/api/IzipayWebhook/notification`
   - Activar notificaciones para: `PAID`, `FAILED`, `CANCELLED`

3. **Probar en ambiente de prueba:**
   - Usa credenciales de **TEST** primero
   - Crear reserva → Pagar con Yape/Plin de prueba
   - Verificar que webhook llega correctamente

4. **Migrar base de datos:**
   ```bash
   # Ejecutar script de migración
   sqlcmd -S tu_servidor -d tu_db -i MIGRACION_IZIPAY.sql
   ```

5. **Desplegar a producción:**
   - Cambiar credenciales a **PRODUCCIÓN**
   - Asegurar que webhook URL sea accesible públicamente (no localhost)
   - Monitorear logs para verificar funcionamiento

---

## 🆚 COMPARACIÓN: ANTES VS AHORA

| Aspecto | ANTES (Manual) | AHORA (Izipay) |
|---------|----------------|----------------|
| **QR Code** | JSON genérico | QR oficial de Yape/Plin |
| **Apertura de app** | Manual | Automática |
| **Verificación** | Sin validación real | Automática vía webhook |
| **Riesgo de fraude** | ❌ Alto | ✅ Cero |
| **Trabajo operador** | ❌ Debe verificar manualmente | ✅ Automático |
| **Experiencia UX** | ⭐⭐ Regular | ⭐⭐⭐⭐⭐ Excelente |
| **Comisión** | 0% | 3.5-4.5% |

---

**Última actualización:** 2025-10-31
**Estado:** ✅ YAPE/PLIN (Izipay) + EFECTIVO MANUAL
**Versión:** 2.0 - Integración con Izipay
