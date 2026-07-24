# AGENTS.md - Guía para IA en ReservaCanchas API

## Propósito

Este archivo contiene las reglas e instrucciones que la IA debe seguir al trabajar en este proyecto. Es el **orquestador principal** que se carga automáticamente al inicio de cada conversación.

---

## Regla #1: SIEMPRE cargar contexto del proyecto

**OBLIGATORIO**: Antes de cualquier tarea, cargar el skill `dotnet-project-knowledge` que contiene el contexto completo del proyecto:

```
skill("dotnet-project-knowledge")
```

Este skill contiene:
- Arquitectura Clean Architecture + CQRS
- Estructura de capas (Api, Application, Domain, Repository, Entity)
- Tecnologías (MediatR, EF Core, FluentValidation, AutoMapper)
- Constantes de negocio (estados, métodos de pago)
- Flujos principales (reservas, pagos, planes)

---

## Carga de Skills según Tarea

Después de cargar `dotnet-project-knowledge`, cargar el skill específico según la necesidad:

| Tarea | Skills a cargar |
|-------|-----------------|
| Crear nuevo Command/Handler | `dotnet-project-knowledge` → `dotnet-command` |
| Crear nuevo Query/Handler | `dotnet-project-knowledge` → `dotnet-query` |
| Crear Entity o DTO | `dotnet-project-knowledge` → `dotnet-entity` |
| Crear Application Layer | `dotnet-project-knowledge` → `dotnet-application` |
| Crear Controller | `dotnet-project-knowledge` → `dotnet-controller` |
| Crear Validator | `dotnet-project-knowledge` → `dotnet-validator` |
| Crear entidad completa (CRUD) | Todos los skills de arriba |
| Modificar lógica de negocio | `dotnet-project-knowledge` → `dotnet-command` |
| Agregar endpoint | `dotnet-project-knowledge` → `dotnet-controller` → `dotnet-application` |
| Validación con Repository | `dotnet-project-knowledge` → `dotnet-validator` |
| Integración Culqi | `dotnet-project-knowledge` → `culqi-integration` |

---

## Skills Disponibles

| Skill | Ubicación | Propósito |
|-------|-----------|-----------|
| `dotnet-project-knowledge` | `.opencode/skills/dotnet-project-knowledge/` | Conocimiento completo del proyecto (SIEMPRE cargar) |
| `dotnet-command` | `.opencode/skills/dotnet-command/` | Commands, Handlers, Validators para CQRS |
| `dotnet-query` | `.opencode/skills/dotnet-query/` | Queries, Handlers para búsquedas paginadas |
| `dotnet-entity` | `.opencode/skills/dotnet-entity/` | Entities, DTOs, Models |
| `dotnet-application` | `.opencode/skills/dotnet-application/` | Application Layer (Facade pattern) |
| `dotnet-controller` | `.opencode/skills/dotnet-controller/` | API Controllers (pass-through) |
| `dotnet-validator` | `.opencode/skills/dotnet-validator/` | FluentValidation patterns |

---

## Arquitectura del Proyecto

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

---

## Convenciones de Nombres

### Para crear una nueva entidad completa:

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Entity | `PascalCase` singular | `MiEntidad.cs` |
| ID Principal | `Id[Entity]` | `IdMiEntidad` |
| ID Foránea | `Id[ReferencedEntity]` | `IdCategoria` |
| Navegación | `Id[Entity]Navigation` | `IdCategoriaNavigation` |
| Collection | `Plural` | `MiEntidadDetalle` |
| Soft Delete | `bool Activo` | `Activo` |

### Para DTOs:

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Base DTO | `[Entity]Dto.cs` | `MiEntidadDto.cs` |
| Create | `Create[Entity]Dto.cs` | `CreateMiEntidadDto.cs` |
| Get | `Get[Entity]Dto.cs` | `GetMiEntidadDto.cs` |
| Update | `Update[Entity]Dto.cs` | `UpdateMiEntidadDto.cs` |
| List | `List[Entity]Dto.cs` | `ListMiEntidadDto.cs` |
| Select | `Select[Entity]Dto.cs` | `SelectMiEntidadDto.cs` |
| Search Filter | `Search[Entity]FilterDto.cs` | `SearchMiEntidadFilterDto.cs` |
| SelectCombo | `SelectCombo[Entity]Dto.cs` | `SelectComboMiEntidadDto.cs` |

### Para Commands/Queries:

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Command | `[Acción][Entity]Command.cs` | `CreateMiEntidadCommand.cs` |
| Handler | `[Acción][Entity]CommandHandler.cs` | `CreateMiEntidadCommandHandler.cs` |
| Validator | `[Acción][Entity]CommandValidator.cs` | `CreateMiEntidadCommandValidator.cs` |
| Query Get | `Get[Entity]Query.cs` | `GetMiEntidadQuery.cs` |
| Query Search | `Search[Entity]Query.cs` | `SearchMiEntidadQuery.cs` |
| Handler Get | `Get[Entity]QueryHandler.cs` | `GetMiEntidadQueryHandler.cs` |
| Handler Search | `Search[Entity]QueryHandler.cs` | `SearchMiEntidadQueryHandler.cs` |

---

## Flujo de Trabajo para Nuevos Requerimientos

### Paso 1: Cargar Contexto
1. Cargar `skill("dotnet-project-knowledge")` - SIEMPRE primero
2. Cargar skill específico según la tarea

### Paso 2: Entender el Requerimiento
1. Leer completamente el requerimiento del usuario
2. Identificar si es: Command, Query, Entity, DTO, Controller, etc.
3. Determinar qué entidades existentes se relacionan

### Paso 3: Buscar Patrones Existentes
1. Revisar skills cargados
2. Revisar `Docs/NEGOCIO.md` para contexto de negocio
3. Revisar `Docs/ARQUITECTURA.md` para estructura
4. Si hay duda, buscar en `Reserva.Domain/Commands/` o `Reserva.Domain/Queries/` si ya existe algo similar

### Paso 4: Implementar Siguiendo el Skill
1. Seguir las reglas de los skills modulares
2. Usar las convenciones de nombres
3. Implementar validación con FluentValidation
4. Usar Repository Pattern para acceso a datos
5. Usar AutoMapper para mapeo de objetos
6. Retornar ResponseDto siempre

### Paso 5: Verificar
1. Ejecutar `dotnet build` para verificar errores de compilación
2. Ejecutar `dotnet test` si hay tests
3. Verificar que no hay errores
4. Verificar que las dependencias están correctamente inyectadas

---

## Documentación del Proyecto

| Documento | Ubicación | Propósito |
|-----------|-----------|-----------|
| NEGOCIO.md | `Docs/NEGOCIO.md` | Contexto de negocio y flujos |
| ARQUITECTURA.md | `Docs/ARQUITECTURA.md` | Arquitectura técnica del sistema |
| RECOMENDACIONES_BACKEND.md | `Docs/RECOMENDACIONES_BACKEND.md` | Recomendaciones de implementación |
| FLUJO_PAGO_CULQI.md | `Docs/FLUJO_PAGO_CULQI.md` | Integración con Culqi |
| DOCUMENTACION_PLANES_SAAS.md | `Docs/DOCUMENTACION_PLANES_SAAS.md` | Sistema de planes SaaS |

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

## Errores Comunes a Evitar

1. **NO crear transacciones manuales** → Usar `UnitOfWork.ExecuteInTransactionAsync()`
2. **NO setear auditoría manualmente** → El UnitOfWork lo hace automáticamente
3. **NO olvidar Include en Queries** → Siempre incluir navegaciones con `IncludeProperties`
4. **NO usar LINQ dinámico** → Problemas de traducción con EF Core
5. **NO exponer claves secretas** → Solo `PublicKey` en frontend, `SecretKey` en backend
6. **NO crear validators manuales para Search** → `SearchQueryValidatorBase` se aplica automáticamente
7. **NO registrar services manualmente** → Scrutor lo hace automáticamente
8. **NO olvidar Soft Delete** → Usar `entidad.Activo = false` en vez de delete físico
9. **NO usar `any`** → Siempre tipar variables y parámetros
10. **NO ignorar errores** → Siempre manejar excepciones

---

## Archivos de Ejemplo por Patrón

Cuando necesites implementar algo, busca el ejemplo más cercano:

| Necesidad | Archivo de Referencia |
|-----------|----------------------|
| Command Create | `Reserva.Domain/Commands/Dbo/Cancha/CreateCanchaCommand.cs` |
| Command Delete | `Reserva.Domain/Commands/Dbo/Reserva/DeleteReservaCommand.cs` |
| Query Get | `Reserva.Domain/Queries/Dbo/Reserva/GetReservaQuery.cs` |
| Query Search | `Reserva.Domain/Queries/Dbo/Reserva/SearchReservaQuery.cs` |
| Entity | `Reserva.Entity/Cancha.cs` |
| DTO Create | `Reserva.Dto/Dbo/Cancha/CreateCanchaDto.cs` |
| DTO Get | `Reserva.Dto/Dbo/Cancha/GetCanchaDto.cs` |
| Application | `Reserva.Application/Dbo/CanchaApplication.cs` |
| Controller | `Reserva.Api/Controllers/Dbo/CanchaController.cs` |
| Validator | `Reserva.Domain/Commands/Dbo/Cancha/CreateCanchaCommandValidator.cs` |

---

## Checklist Pre-Entrega

Antes de entregar cualquier implementación:

- [ ] Se cargó `skill("dotnet-project-knowledge")` primero
- [ ] Se cargó el skill específico para la tarea
- [ ] Entity tiene `Activo` para Soft Delete
- [ ] Entity tiene campos de auditoría (`UserNameCreate`, `CreateDate`, etc.)
- [ ] DTOs están correctamente tipados
- [ ] Command/Query extiende la base correcta
- [ ] Handler implementa `HandleCommand` o `HandleQuery`
- [ ] Validator extiende `CommandValidatorBase` o `QueryValidatorBase`
- [ ] Application Layer implementa la interface
- [ ] Controller implementa la interface de Application
- [ ] No hay errores de compilación (`dotnet build`)
- [ ] No hay código duplicado
- [ ] Se siguió la convención de nombres

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
