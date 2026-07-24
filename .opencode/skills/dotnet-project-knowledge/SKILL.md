---
name: dotnet-project-knowledge
description: Conocimiento completo del proyecto ReservaCanchas API - Arquitectura, patrones, tecnologías y contexto de negocio
---

# ReservaCanchas API - Conocimiento del Proyecto

## Información General

**Nombre**: ReservaCanchas API
**Tipo**: REST API para gestión de reservas de canchas deportivas
**Arquitectura**: Clean Architecture + CQRS
**Lenguaje**: C# (.NET 8)
**Base de Datos**: SQL Server + MySQL

---

## Arquitectura del Proyecto

### Estructura de Capas

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

## Conceptos de Negocio Importantes

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

## Decisiones Técnicas Importantes

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

### 2. Response Pattern

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

### 3. Repository Pattern

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

### 4. Auditoría Automática

Si una entidad tiene estas propiedades, se auditan automáticamente:
```csharp
public string UserNameCreate { get; set; }
public DateTimeOffset CreateDate { get; set; }
public string? UserNameUpdate { get; set; }
public DateTimeOffset? UpdateDate { get; set; }
public bool Activo { get; set; }
```

**NO setearlas manualmente** - el UnitOfWork las actualiza automáticamente.

### 5. Transacciones

Los `CommandHandlers` ejecutan automáticamente en transacción vía `UnitOfWork`:
- Commit automático si todo OK
- Rollback automático si hay error
- Reintentos automáticos en caso de conflictos de concurrencia (3 intentos)

**NO crear transacciones manuales** a menos que sea absolutamente necesario.

---

## Integraciones Externas

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

**Limitaciones importantes**:
- Yape: Máximo S/ 2,000 por transacción
- Código de aprobación Yape: Válido 2 minutos
- Moneda: Solo PEN (soles)

---

## Estructura de Archivos Importante

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

---

## Endpoints Estándar

```
GET    /api/[Entity]/{id}           → Obtener por ID
POST   /api/[Entity]                → Crear
PUT    /api/[Entity]                → Actualizar
DELETE /api/[Entity]/{id}           → Eliminar
POST   /api/[Entity]/search         → Búsqueda paginada
GET    /api/[Entity]/SelectCombo    → Catálogo para selects
GET    /api/[Entity]/Select         → Selección simple
POST   /api/[Entity]/list           → Listado
```

---

## Tecnologías y Versiones

### Framework y Core
| Tecnología | Versión | Uso |
|------------|---------|-----|
| .NET | 8.0 | Framework principal |
| C# | 12.0 | Lenguaje |
| Entity Framework Core | 8.0 | ORM |
| MediatR | 12.0 | CQRS |
| FluentValidation | 11.0 | Validación |
| AutoMapper | 12.0 | Mapeo |

### Seguridad
| Tecnología | Versión | Uso |
|------------|---------|-----|
| JWT Bearer | 8.0 | Autenticación |
| ASP.NET Core Identity | 8.0 | Gestión de usuarios |

### Base de Datos
| Tecnología | Versión | Uso |
|------------|---------|-----|
| SQL Server | 2019+ | Base de datos principal |
| MySQL | 8.0+ | Base de datos secundaria |

---

## Constantes de Negocio

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

## Limitaciones y Restricciones Conocidas

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

## Configuración de Seguridad

### JWT Tokens
- `UserIdNegocio` claim contiene IdProveedor o IdOperador (según rol)
- `IUserIdetity` service para leer claims del JWT

### Autorización
```csharp
[Authorize]  // Solo usuarios autenticados
[Authorize(Roles = "Admin")]  // Solo usuarios con rol Admin
```

### Claims de Usuario
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userName = User.FindFirst(ClaimTypes.Name)?.Value;
```

---

## Comandos Útiles

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

## Última Actualización

**Fecha**: 2026-07-23
**Versión del proyecto**: 1.0
**Mantenedor**: Equipo ReservaCanchas
