# 🧠 Memoria del Proyecto - ReservaCanchas API

> Este archivo contiene información que Claude debe recordar entre sesiones sobre el proyecto.

---

## 📋 Información General del Proyecto

**Nombre**: ReservaCanchas API
**Tipo**: REST API para gestión de reservas de canchas deportivas
**Arquitectura**: Clean Architecture + CQRS
**Lenguaje**: C# (.NET)
**Base de Datos**: SQL Server

---

## 🏗️ Arquitectura del Proyecto

### Estructura de Capas (Orden de dependencia)

```
┌─────────────────────────────────────────┐
│         Reserva.Api (Presentación)      │
├─────────────────────────────────────────┤
│      Reserva.Application (Orquestación) │
├─────────────────────────────────────────┤
│    Reserva.Domain (Lógica de Negocio)   │
├─────────────────────────────────────────┤
│    Reserva.Repository (Acceso a Datos)  │
├─────────────────────────────────────────┤
│         Reserva.Entity (Modelos)        │
└─────────────────────────────────────────┘
         Reserva.Dto (Transfer Objects)
```

**REGLA CRÍTICA**: Las capas inferiores NUNCA referencian capas superiores.

### Tecnologías y Patrones

| Categoría | Tecnología/Patrón |
|-----------|-------------------|
| **Arquitectura** | Clean Architecture + CQRS |
| **ORM** | Entity Framework Core |
| **Mediator** | MediatR |
| **Validación** | FluentValidation |
| **Mapping** | AutoMapper |
| **Transacciones** | Unit of Work Pattern |
| **Acceso a Datos** | Repository Pattern |
| **Lógica Variable** | Strategy Pattern |

---

## 🎯 Conceptos de Negocio Importantes

### 1. Separación de Responsabilidades: Reservas vs Planes

**⚠️ CRÍTICO - NO MEZCLAR**:

| Concepto | Método de Pago | Confirmación | Sistema |
|----------|----------------|--------------|---------|
| **Reservas de Canchas** | EFECTIVO únicamente | Manual (operador/admin confirma) | Sistema de reservas existente |
| **Planes de Proveedores** | Culqi (Yape/Plin/Tarjetas) | Automática (webhook) | Sistema Culqi implementado |

**Por qué**:
- Reservas son transacciones cliente → proveedor (el proveedor confirma el efectivo)
- Planes son transacciones proveedor → plataforma (la plataforma cobra por servicios premium)

### 2. Flujo de Reserva

```
Usuario selecciona cancha + horario
    ↓
Valida disponibilidad
    ↓
Crea Reserva (Estado: PENDIENTE)
    ↓
Crea Pago (Método: EFECTIVO, Estado: PENDIENTE)
    ↓
Usuario/Proveedor se encuentran
    ↓
Operador confirma pago en efectivo
    ↓
Pago → PAGADO
Reserva → CONFIRMADA
```

### 3. Estados del Sistema

**Estados de Reserva** (`Constants.ESTADO_RESERVA`):
- `Pendiente` (PE) - Creada, esperando confirmación de pago
- `Confirmado` (CO) - Pago confirmado, reserva activa
- `Cancelado` (CA) - Reserva cancelada

**Estados de Pago** (`Constants.ESTADO_PAGO`):
- `Pendiente` (02) - Esperando pago
- `Parcial` (04) - Adelanto pagado (solo efectivo)
- `Pagado` (01) - Pago completo
- `Rechazado` (03) - Pago rechazado/fallido

**Métodos de Pago** (`Constants.METODO_PAGO`):
- `Efectivo` (02) - Para reservas
- `Yape` (04) - Para planes (vía Culqi)
- `Plin` (05) - Para planes (vía Culqi)
- `Transferencia` (03) - Para planes (vía Culqi)

### 4. Adelantos (Solo Efectivo)

- **Porcentaje mínimo**: 50% (configurable en `appsettings.json`)
- **Comportamiento**:
  - Si adelanto ≥ 50% del total → Reserva se confirma automáticamente
  - Si adelanto < 50% → Reserva queda PENDIENTE
  - Otros métodos de pago NO permiten adelantos

---

## 🔧 Decisiones Técnicas Importantes

### 1. CQRS - Separación Commands/Queries

**Commands** (Modifican datos):
- `CreateReservaCommand` → `CreateReservaCommandHandler`
- `UpdateTelefonoCommand` → `UpdateTelefonoCommandHandler`
- Ejecutan en transacción automática
- Requieren validación con FluentValidation

**Queries** (Solo lectura):
- `GetReservaQuery` → `GetReservaQueryHandler`
- `SearchCanchaQuery` → `SearchCanchaQueryHandler`
- No modifican datos
- No necesitan transacción

### 2. Convenciones de Nombres

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Command | `[Acción][Entity]Command.cs` | `CreateReservaCommand.cs` |
| Handler | `[Acción][Entity]CommandHandler.cs` | `CreateReservaCommandHandler.cs` |
| Validator | `[Acción][Entity]CommandValidator.cs` | `CreateReservaCommandValidator.cs` |
| Query | `[Operación][Entity]Query.cs` | `GetReservaQuery.cs` |
| DTO | `[Operación][Entity]Dto.cs` | `CreateReservaDto.cs` |
| ID Primaria | `Id[Entity]` | `IdReserva`, `IdCancha` |
| ID Foránea | `Id[ReferencedEntity]` | `IdProveedor`, `IdEstadoReserva` |
| Navegación | `Id[Entity]Navigation` | `IdCanchaNavigation` |

### 3. Validación con FluentValidation

```csharp
// Patrón estándar
public class CreateReservaCommandValidator : CommandValidatorBase<CreateReservaCommand>
{
    public CreateReservaCommandValidator(...)
    {
        RequiredInformation(x => x.CreateDto).DependentRules(() =>
        {
            RuleFor(x => x.CreateDto.IdCancha)
                .MustAsync(ValidarExistencia)
                .WithMessage("La cancha no existe");

            RuleFor(x => x.CreateDto.Monto)
                .GreaterThan(0)
                .WithMessage("El monto debe ser mayor a 0");
        });
    }
}
```

### 4. Response Pattern

**SIEMPRE** usar `ResponseDto` o `ResponseDto<T>`:

```csharp
var response = new ResponseDto<ReservaDto>();

// Éxito
response.UpdateData(reserva);
response.AddOkResult("Reserva creada exitosamente");

// Error
response.AddErrorResult("Error al crear reserva");

return response;
```

### 5. Repository Pattern

**Métodos comunes**:
```csharp
// Una entidad
await _repository.GetByAsync(x => x.IdReserva == id,
    x => x.IdCanchaNavigation,  // Include navegación
    x => x.ReservaDetalle       // Include colección
);

// Múltiples entidades
await _repository.FindByAsync(x => x.Activo);

// Búsqueda paginada
await _repository.SearchByAsync(page, pageSize, sort, filter);

// Crear
await _repository.AddAsync(entidad);
await _repository.SaveAsync();

// Actualizar
await _repository.UpdateAsync(entidad);
await _repository.SaveAsync();
```

### 6. Auditoría Automática

Si una entidad tiene estas propiedades, se auditan automáticamente:
```csharp
public string UserNameCreate { get; set; }
public DateTimeOffset CreateDate { get; set; }
public string? UserNameUpdate { get; set; }
public DateTimeOffset? UpdateDate { get; set; }
public bool Activo { get; set; }
```

**NO setearlas manualmente** - el UnitOfWork las actualiza automáticamente.

### 7. Transacciones

Los `CommandHandlers` ejecutan automáticamente en transacción vía `UnitOfWork`:
- Commit automático si todo OK
- Rollback automático si hay error
- Reintentos automáticos en caso de conflictos de concurrencia (3 intentos)

**NO crear transacciones manuales** a menos que sea absolutamente necesario.

---

## 🔌 Integraciones Externas

### Culqi (Pasarela de Pagos)

**Propósito**: Procesar pagos de planes de proveedores (NO reservas)

**Configuración**:
- Testing: `https://integ-panel.culqi.com`
- Producción: `https://panel.culqi.com`
- Claves en: `appsettings.json` → `Culqi` section

**Componentes**:
- `CulqiService` - Servicio principal en `Reserva.Domain/Services/Culqi/`
- `CulqiWebhookController` - Endpoint: `/api/culqi/webhook`
- DTOs: `CulqiCreateChargeRequest`, `CulqiChargeResponse`, `CulqiWebhookEvent`

**Campos en tabla Pago**:
- `CulqiChargeId` - ID del cargo en Culqi
- `CulqiTokenId` - Token del frontend
- `CulqiReferenceCode` - Código de referencia

**Webhook Events**:
- `charge.succeeded` - Pago con tarjeta exitoso
- `charge.failed` - Pago rechazado
- `order.status.changed` - Para Yape/Plin (QR)

**Limitaciones importantes**:
- Yape: Máximo S/ 2,000 por transacción
- Código de aprobación Yape: Válido 2 minutos
- Moneda: Solo PEN (soles)

**Flujo**:
```
Frontend genera token (CulqiJS)
    ↓
Backend crea cargo con token
    ↓
Culqi procesa pago
    ↓
Webhook notifica resultado
    ↓
Backend actualiza estado automáticamente
```

---

## 🚫 Limitaciones y Restricciones Conocidas

### 1. Métodos de Pago

- ❌ Reservas NO pueden usar Yape/Plin/Tarjetas directamente
- ✅ Reservas SOLO aceptan EFECTIVO (confirmación manual)
- ✅ Planes SÍ usan Culqi (Yape/Plin/Tarjetas)

### 2. Yape vía Culqi

- Monto máximo: S/ 2,000
- Código de aprobación expira en 2 minutos
- Solo soles (PEN)
- No soporta reembolsos automáticos

### 3. Adelantos

- Solo disponibles para método EFECTIVO
- Porcentaje mínimo: 50% (configurable)
- Otros métodos de pago NO permiten adelantos parciales

### 4. Seguridad

- NUNCA exponer claves secretas de Culqi en frontend
- Solo `PublicKey` en cliente, `SecretKey` en backend
- No commitear archivos `.env`, `.env.*`, `appsettings.Production.json`
- No leer/editar archivos con claves privadas (`.key`, `.pfx`)

### 5. Entity Framework

- No usar LINQ dinámico (problemas de traducción)
- Siempre incluir navegaciones con `includeProperties`
- No modificar entidades obtenidas con `AsNoTracking`

---

## 📁 Estructura de Archivos Importante

### Para crear una nueva entidad completa:

```
1. Entity:     Reserva.Entity/[Entity].cs
2. DTO Base:   Reserva.Dto/Dbo/[Entity]/[Entity]Dto.cs
3. DTOs:       Reserva.Dto/Dbo/[Entity]/
               - Create[Entity]Dto.cs
               - Get[Entity]Dto.cs
               - Update[Entity]Dto.cs
               - Search[Entity]Dto.cs
               - Search[Entity]FilterDto.cs
4. Command:    Reserva.Domain/Commands/Dbo/[Entity]/
               - Create[Entity]Command.cs
               - Create[Entity]CommandHandler.cs
               - Create[Entity]CommandValidator.cs
5. Query:      Reserva.Domain/Queries/Dbo/[Entity]/
               - Get[Entity]Query.cs
               - Get[Entity]QueryHandler.cs
6. Application: Reserva.Application/Dbo/[Entity]Application.cs
7. Interface:  Reserva.Application.Abstractions/Dbo/I[Entity]Application.cs
8. Controller: Reserva.Api/Controllers/Dbo/[Entity]Controller.cs
```

### Para servicios externos:

```
Reserva.Domain/Services/[NombreServicio]/
    ├── [Servicio]Service.cs
    ├── [Servicio]Request.cs
    ├── [Servicio]Response.cs
    └── [Servicio]Exception.cs (opcional)
```

---

## 🔑 Constantes de Negocio

Ubicación: `Reserva.Common/Constants.cs`

### Estados de Reserva
```csharp
Constants.ESTADO_RESERVA.Pendiente   // "PE"
Constants.ESTADO_RESERVA.Confirmado  // "CO"
Constants.ESTADO_RESERVA.Cancelado   // "CA"
```

### Estados de Pago
```csharp
Constants.ESTADO_PAGO.Pagado      // "01"
Constants.ESTADO_PAGO.Pendiente   // "02"
Constants.ESTADO_PAGO.Rechazado   // "03"
Constants.ESTADO_PAGO.Parcial     // "04"
```

### Métodos de Pago
```csharp
Constants.METODO_PAGO.Tarjeta        // "01" (no implementado)
Constants.METODO_PAGO.Efectivo       // "02"
Constants.METODO_PAGO.Transferencia  // "03"
Constants.METODO_PAGO.Yape          // "04"
Constants.METODO_PAGO.Plin          // "05"
```

---

## 📚 Documentación del Proyecto

| Archivo | Propósito |
|---------|-----------|
| `FLUJO_PAGO_CULQI.md` | Documentación completa de integración Culqi |
| `README_CULQI_QUICK_START.md` | Guía rápida para empezar con Culqi |
| `MIGRACION_CULQI.sql` | Script de migración para campos Culqi |
| `.claude/README.md` | Documentación de configuración Claude Code |
| `.claude/settings.json` | Reglas y configuración del proyecto |

---

## 🎯 Próximas Funcionalidades Planificadas

### 1. Entidad Plan (No implementada aún)
Para que proveedores contraten planes premium:
- Destacar canchas
- Reportes avanzados
- Mayor visibilidad

**Campos sugeridos**:
```csharp
public class Plan
{
    public int IdPlan { get; set; }
    public string Nombre { get; set; }         // "Plan Basic", "Plan Premium"
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int DuracionDias { get; set; }      // 30, 90, 365
    public bool Activo { get; set; }
    // Auditoría...
}
```

### 2. Activación Automática de Plan
Después del webhook de Culqi, activar plan del proveedor automáticamente.

### 3. Notificaciones
- Email al proveedor cuando se confirma pago del plan
- SMS de confirmación (opcional)

### 4. Dashboard de Proveedores
- Ver estado del plan actual
- Historial de pagos
- Renovación de plan

---

## ⚠️ Errores Comunes y Soluciones

### Error: "Invalid API Key" (Culqi)
**Causa**: Clave incorrecta o de ambiente equivocado
**Solución**: Verificar que `SecretKey` coincida con el ambiente (test vs live)

### Error: Webhook no se recibe
**Causa**: URL no accesible o no configurada
**Solución**:
1. URL debe ser pública (no localhost)
2. Usar HTTPS
3. Configurar en Panel Culqi
4. Para testing local: usar ngrok

### Error: Pago queda en PENDIENTE
**Causa**: Webhook no llegó o falló
**Solución**:
1. Revisar logs del webhook controller
2. Verificar en Panel Culqi si se envió
3. Actualizar manualmente después de verificar

### Error: Transacción duplicada
**Causa**: Webhook llegó múltiples veces
**Solución**: El sistema ya maneja idempotencia - verifica estado actual antes de actualizar

---

## 🔄 Comandos Útiles

### Ejecutar migración Culqi:
```bash
sqlcmd -S tu_servidor -d ReservaCanchas -i MIGRACION_CULQI.sql
```

### Build del proyecto:
```bash
dotnet build
```

### Ejecutar tests:
```bash
dotnet test
```

### Formatear código:
```bash
dotnet format
```

### Ver estado de Git:
```bash
git status
```

---

## 🧪 Testing

### Tarjetas de prueba Culqi:

| Marca | Número | CVV | Fecha | Resultado |
|-------|--------|-----|-------|-----------|
| Visa | 4111 1111 1111 1111 | 123 | 09/25 | Éxito |
| Visa | 4000 0000 0000 0002 | 123 | 09/25 | Rechazo |
| Mastercard | 5111 1111 1111 1118 | 472 | 09/25 | Éxito |

### Ambiente de pruebas:
- Panel: https://integ-panel.culqi.com
- API: https://api.culqi.com

---

## 👥 Contactos y Recursos

### Culqi
- **Email**: soporte@culqi.com
- **Teléfono**: +51 1 644 8495
- **Docs**: https://docs.culqi.com
- **API Reference**: https://apidocs.culqi.com

### Claude Code
- **Docs**: https://docs.claude.com/en/docs/claude-code
- **GitHub Issues**: https://github.com/anthropics/claude-code/issues

---

**Última actualización**: 2025-11-01
**Versión del proyecto**: 1.0
**Mantenedor**: Equipo ReservaCanchas
