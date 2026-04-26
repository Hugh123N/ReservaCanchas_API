# ReservaCanchas API - Documentacion de Arquitectura

---

## 1. VISION GENERAL

El proyecto **ReservaCanchas** es una plataforma de reserva de canchas deportivas construida en **C# .NET 8**. El backend implementa una arquitectura moderna por capas que combina varios patrones de diseño reconocidos de la industria.

El proyecto se divide en dos grandes zonas:

| Zona | Descripcion |
|------|-------------|
| **Carpeta `dbo/`** | Todo lo que es **logica de negocio**: entidades, DTOs, comandos, queries, aplicacion |
| **Fuera de `dbo/`** | Todo lo que es **infraestructura/arquitectura**: repositorio base, contexto de BD, servicios transversales, configuracion |

---

## 1.1. PATRONES DE ARQUITECTURA Y DISEÑO

El proyecto no usa un unico patron, sino una combinacion pensada de varios:

### Clean Architecture (Arquitectura Limpia)
Propuesta por Robert C. Martin (Uncle Bob). Organiza el codigo en capas concentricas donde **las capas internas no saben nada de las capas externas**. En este proyecto se expresa como proyectos separados donde la dependencia siempre apunta hacia adentro:

```
Api → Application → Domain ← Repository ← Entity
```

La capa `Domain` (negocio) no depende de nada de infraestructura. Si maniana cambia la base de datos de SQL Server a PostgreSQL, el dominio no se toca.

### Hexagonal Architecture (Arquitectura Hexagonal / Ports & Adapters)
Propuesta por Alistair Cockburn. Define que el nucleo del negocio se comunica con el exterior a traves de **puertos (interfaces)** y **adaptadores (implementaciones)**. En este proyecto se implementa mediante los proyectos `*.Abstractions`:

| Puerto (Interfaz) | Adaptador (Implementacion) |
|-------------------|---------------------------|
| `Reserva.Repository.Abstractions` → `IRepository<T>` | `Reserva.Repository` → `Repository<T>` |
| `Reserva.Application.Abstractions` → `ICanchaApplication` | `Reserva.Application` → `CanchaApplication` |

El dominio solo conoce las interfaces, nunca las implementaciones concretas.

### DDD — Domain-Driven Design (Diseño Orientado al Dominio)
Propuesto por Eric Evans. Organiza el codigo alrededor del **lenguaje del negocio**, no alrededor de la tecnologia. En este proyecto se aplica:

- **Lenguaje ubicuo**: Los nombres en el codigo reflejan el negocio (`Cancha`, `Reserva`, `Proveedor`, `HorarioCancha`, `BloqueoHorario`)
- **Agregados**: Cada entidad agrupa sus relaciones (`Cancha` contiene `HorarioCancha[]`, `ImagenCancha[]`, etc.)
- **Value Objects implícitos**: Las constantes de negocio en `Reserva.Common/Constants.cs` representan valores del dominio (`ESTADO_CANCHA`, `ESTADO_RESERVA`, `METODO_PAGO`)
- **Domain Services**: Servicios que encapsulan logica que no pertenece a una sola entidad (`HorarioCanchaService`, `CulqiService`)
- **Repositorios**: Abstraccion del acceso a datos desde el dominio

### CQRS — Command Query Responsibility Segregation
Propuesto por Greg Young (basado en CQS de Bertrand Meyer). **Separa las operaciones de escritura (Commands) de las de lectura (Queries)**. En este proyecto:

| Tipo | Responsabilidad | Ejemplo |
|------|-----------------|---------|
| **Command** | Modifica datos, ejecuta en transaccion | `CreateCanchaCommand`, `UpdateReservaCommand` |
| **Query** | Solo lee datos, sin efectos secundarios | `GetCanchaQuery`, `SearchCanchaQuery` |

Esto permite optimizar cada lado de forma independiente (ej: queries con `AsNoTracking` para mejor rendimiento).

### Mediator Pattern (Patron Mediador)
Implementado con la libreria **MediatR**. En lugar de que el controlador llame directamente al servicio, envia un mensaje a un mediador que lo enruta al handler correcto. Desacopla completamente los controladores de la logica de negocio:

```
Controller → IMediator.Send(Command) → CommandHandler
                                     → QueryHandler
```

### Repository Pattern (Patron Repositorio)
Abstrae el acceso a la base de datos. Los handlers del dominio no usan `DbContext` directamente, sino un `IRepository<TEntity>` generico. Esto permite:
- Cambiar el ORM sin tocar el dominio
- Facilitar pruebas unitarias con mocks

### Unit of Work Pattern (Patron Unidad de Trabajo)
Agrupa multiples operaciones de base de datos en una sola transaccion atomica. En este proyecto el `UnitOfWork` es automatico: todos los `CommandHandlers` ejecutan dentro de una transaccion sin que el desarrollador deba gestionarla manualmente.

### Strategy Pattern (Patron Estrategia)
Permite cambiar comportamientos en tiempo de ejecucion. Se usa en el sistema de horarios (`HorarioCanchaService`) y en el sistema de pagos, donde cada metodo de pago puede tener su propia estrategia de procesamiento.

---

## 1.2. DEPENDENCIAS (PAQUETES NUGET)

### `Reserva.Api` — Presentacion

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.0 | Valida tokens JWT en los endpoints protegidos. Extrae el usuario del token automaticamente |
| `Swashbuckle.AspNetCore` | 6.4.0 | Genera la documentacion Swagger/OpenAPI automaticamente desde los controladores |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.4 | Herramienta de diseno para generar migraciones y scaffolding (solo desarrollo) |

---

### `Reserva.Domain` — Logica de Negocio

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `MediatR` | 12.5.0 | Implementa el patron Mediador. Enruta Commands y Queries a sus Handlers. Desacopla controladores de logica |
| `AutoMapper` | 14.0.0 | Mapea automaticamente propiedades entre objetos (Entity → DTO, DTO → Entity) sin escribir asignaciones manuales |
| `MailKit` | 4.13.0 | Libreria para enviar correos electronicos (notificaciones de reserva, confirmaciones, etc.) |
| `AWSSDK.S3` | 4.0.14.1 | Cliente para Amazon S3. Sube y gestiona las imagenes de las canchas en la nube |
| `Google.Apis.Auth` | 1.70.0 | Valida tokens de Google OAuth para el login con cuenta Google |
| `Microsoft.AspNetCore.Identity` | 2.3.1 | Provee el sistema de hashing de passwords y gestion de usuarios |
| `Scrutor` | 6.0.1 | Escanea ensamblados y registra servicios automaticamente en el contenedor DI (evita registrar cada clase manualmente) |
| `System.Linq.Dynamic.Core` | 1.6.2 | Permite construir expresiones LINQ dinamicas desde strings (usado para ordenamiento y filtros en busquedas) |
| `RandomStringCreator` | 2.0.0 | Genera strings aleatorios seguros (usado para generar codigos unicos de canchas y reservas) |
| `Microsoft.Extensions.Http` | 9.0.0 | Provee `IHttpClientFactory` para hacer llamadas HTTP a APIs externas (Culqi) de forma segura |
| `Microsoft.Extensions.Configuration.Binder` | 9.0.4 | Permite enlazar secciones del `appsettings.json` a clases de configuracion fuertemente tipadas |

---

### `Reserva.Application` — Orquestacion

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `MediatR` | 12.5.0 | Mismo que en Domain. Usado para enviar comandos/queries desde los servicios de aplicacion |
| `AutoMapper` | 14.0.0 | Mismo que en Domain. Mapeos adicionales a nivel de aplicacion |

---

### `Reserva.Repository` — Acceso a Datos

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `Scrutor` | 6.0.1 | Registro automatico de repositorios en el contenedor DI |

---

### `Reserva.Repository.Abstractions` — Contratos de Repositorio

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `System.Data.SqlClient` | 4.9.0 | Cliente ADO.NET para SQL Server. Usado para ejecutar procedimientos almacenados directamente |

---

### `Reserva.Entity` — Entidades

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.4 | Proveedor de EF Core para SQL Server. Traduce LINQ a SQL de SQL Server |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.4 | Herramientas de diseno: scaffolding de base de datos existente hacia clases C# |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.4 | Comandos CLI para migraciones (`dotnet ef migrations add`, `dotnet ef database update`) |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 6.0.8 | Integra ASP.NET Identity con EF Core para persistir usuarios, roles y claims |
| `Pomelo.EntityFrameworkCore.MySql` | 9.0.0 | Proveedor de EF Core para MySQL/MariaDB (soporte multi-base de datos) |

---

### `Reserva.Common` — Utilidades Comunes

| Paquete | Version | Para que sirve |
|---------|---------|----------------|
| `ClosedXML` | 0.105.0 | Crea y lee archivos Excel (.xlsx) para reportes y exportacion de datos |
| `QRCoder` | 1.7.0 | Genera codigos QR (usado para comprobantes de pago y confirmaciones de reserva) |

---

> **Nota sobre FluentValidation**: La validacion de comandos se implementa con una clase base `CommandValidatorBase<T>` que integra validaciones en el pipeline de MediatR. Cada comando tiene su propio validator con reglas de negocio especificas usando la sintaxis `RuleFor().NotEmpty()`, `RuleFor().MustAsync()`, etc.

---

## 2. CAPAS DEL PROYECTO (Backend)

```
┌────────────────────────────────────────────────────────┐
│              Reserva.Api                               │
│   Controllers/Dbo/  ← Endpoints HTTP del negocio      │
│   Program.cs        ← Inyeccion de dependencias       │
├────────────────────────────────────────────────────────┤
│              Reserva.Application                       │
│   Dbo/              ← Orquestacion de casos de uso    │
├────────────────────────────────────────────────────────┤
│              Reserva.Domain                            │
│   Commands/Dbo/     ← Logica de escritura (CQRS)      │
│   Queries/Dbo/      ← Logica de lectura (CQRS)        │
│   Services/         ← Integraciones externas          │
│   Extensions/       ← Metodos de extension            │
│   Mapping/          ← Perfiles AutoMapper             │
├────────────────────────────────────────────────────────┤
│              Reserva.Repository                        │
│   Base/             ← Repositorio generico            │
│   Data/             ← DbContext (EF Core)             │
│   Transactions/     ← UnitOfWork                      │
│   Security/         ← Contexto de usuario autenticado │
├────────────────────────────────────────────────────────┤
│              Reserva.Entity                            │
│   *.cs              ← Entidades de base de datos      │
├────────────────────────────────────────────────────────┤
│              Reserva.Dto                               │
│   Dbo/              ← Objetos de transferencia (DTO)  │
├────────────────────────────────────────────────────────┤
│              Reserva.Common                            │
│   Constants.cs      ← Constantes del negocio          │
└────────────────────────────────────────────────────────┘
```

**Regla fundamental**: Las capas inferiores **nunca** referencian capas superiores.

---

## 3. DETALLE DE CADA CAPA

### Capa 1: `Reserva.Api` — Presentacion

**Responsabilidad**: Exponer los endpoints HTTP al mundo exterior.

**Dentro de `dbo/`** (negocio):
```
Controllers/
└── Dbo/
    ├── CanchaController.cs
    ├── ReservaController.cs
    ├── ProveedorController.cs
    └── ...
```

**Fuera de `dbo/`** (arquitectura):
```
Program.cs               ← Configuracion de la app, DI, middlewares
appsettings.json         ← Configuracion de conexion, JWT, etc.
```

Los controladores **no tienen logica de negocio**. Implementan la interfaz de aplicacion y delegan al servicio correspondiente:

```csharp
[ApiController]
[Route("api/Cancha")]
[Security.Authorize]
public class CanchaController : ICanchaApplication
{
    private readonly ICanchaApplication _CanchaApplication;

    public CanchaController(ICanchaApplication CanchaApplication)
    {
        _CanchaApplication = CanchaApplication;
    }

    [HttpPost]
    public async Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto)
        => await _CanchaApplication.Create(createDto);

    [HttpGet("{id}")]
    public async Task<ResponseDto<GetCanchaDto>> Get(int id)
        => await _CanchaApplication.Get(id);

    [HttpPost("search")]
    public async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams)
        => await _CanchaApplication.Search(searchParams);
}
```

---

### Capa 2: `Reserva.Application` — Orquestacion

**Responsabilidad**: Coordinar el flujo de un caso de uso (no tiene logica de negocio propia).

**Dentro de `dbo/`** (negocio):
```
Dbo/
├── Cancha/
│   ├── CreateCanchaApplicationService.cs
│   ├── UpdateCanchaApplicationService.cs
│   ├── DeleteCanchaApplicationService.cs
│   ├── GetCanchaApplicationService.cs
│   └── SearchCanchaApplicationService.cs
└── ...
```

Cada servicio de aplicacion:
1. Recibe el comando o query
2. Puede validar permisos o enriquecer datos
3. Delega al handler del dominio via MediatR

---

### Capa 3: `Reserva.Domain` — Logica de Negocio

**Responsabilidad**: Contiene toda la logica del negocio. Es el corazon del sistema.

Se divide en dos grupos segun el patron **CQRS**:

#### Commands (escritura) — `Commands/Dbo/`
```
Commands/Dbo/
└── Cancha/
    ├── CreateCanchaCommand.cs          ← Define el contrato (que datos necesito)
    ├── CreateCanchaCommandValidator.cs ← Valida los datos con FluentValidation
    ├── CreateCanchaCommandHandler.cs   ← Ejecuta la logica
    ├── UpdateCanchaCommand.cs
    ├── UpdateCanchaCommandValidator.cs
    ├── UpdateCanchaCommandHandler.cs
    ├── DeleteCanchaCommand.cs
    └── DeleteCanchaCommandHandler.cs
```

#### Queries (lectura) — `Queries/Dbo/`
```
Queries/Dbo/
└── Cancha/
    ├── GetCanchaQuery.cs               ← Obtener una cancha por ID
    ├── GetCanchaQueryHandler.cs
    ├── ListCanchaQuery.cs              ← Listar todas las canchas del proveedor
    ├── ListCanchaQueryHandler.cs
    ├── SearchCanchaQuery.cs            ← Busqueda avanzada con filtros y paginacion
    ├── SearchCanchaQueryHandler.cs
    ├── SelectCanchaQuery.cs            ← Para llenar combos/dropdowns
    └── SelectCanchaQueryHandler.cs
```

**Fuera de `dbo/`** (arquitectura transversal):
```
Services/
├── HorarioCanchaService.cs    ← Logica de expansion de horarios
└── Culqi/                     ← Integracion con pasarela de pagos

Extensions/                    ← Metodos de extension genericos
Mapping/                       ← Perfiles AutoMapper (entity ↔ DTO)
Resources/                     ← Mensajes de localizacion
```

---

### Capa 4: `Reserva.Repository` — Acceso a Datos

**Responsabilidad**: Abstraer el acceso a la base de datos.

**Fuera de `dbo/`** (todo es infraestructura):
```
Base/
└── Repository.cs               ← Repositorio generico con todos los metodos CRUD

Data/
└── ReservaCanchasContext.cs    ← DbContext de Entity Framework Core

Transactions/
└── UnitOfWork.cs               ← Manejo automatico de transacciones

Security/
└── AuthenticatedUserContext.cs ← Quien esta haciendo la operacion

Extensions/
└── QueryExtensions.cs          ← Helpers para paginacion y filtros
```

El repositorio generico provee:

```csharp
// Lectura
GetAsync(keyValue, includeProperties)
GetByAsync(filter, includeProperties)
FindByAsync(filter, includeProperties)
SearchByAsync(page, pageSize, sorts, filter, includeProperties) // con paginacion

// Escritura
AddAsync(entity)
UpdateAsync(entity)
DeleteAsync(entity)
SaveAsync()

// Procedimientos almacenados
ExecuteScalarSPAsync<T>(spName, parameters)
```

---

### Capa 5: `Reserva.Entity` — Entidades

**Responsabilidad**: Definir el modelo de datos que mapea con la base de datos.

```
Reserva.Entity/
├── Cancha.cs
├── Reserva.cs
├── Proveedor.cs
├── HorarioCancha.cs
├── ImagenCancha.cs
└── ...
```

Estas clases son **auto-generadas** desde la base de datos con EF Core (database-first).

---

### Capa 6: `Reserva.Dto` — Objetos de Transferencia

**Responsabilidad**: Definir que datos entran y salen de la API (nunca exponer la entidad directo).

**Dentro de `dbo/`** (negocio):
```
Dbo/
└── Cancha/
    ├── CanchaDto.cs           ← DTO base con campos comunes
    ├── CreateCanchaDto.cs     ← Datos para crear
    ├── UpdateCanchaDto.cs     ← Datos para actualizar
    ├── GetCanchaDto.cs        ← Datos de respuesta detallada
    └── SearchCanchaDto.cs     ← Datos de respuesta en busqueda
```

**Fuera de `dbo/`** (arquitectura):
```
Base/
├── ResponseDto.cs             ← Respuesta estandar de todos los endpoints
├── CommandBase.cs             ← Contrato base para todos los comandos
└── QueryBase.cs               ← Contrato base para todas las queries
```

---

### Capa 7: `Reserva.Common` — Constantes

**Responsabilidad**: Centralizar las constantes del negocio para evitar strings/numeros magicos.

```
Constants.cs

ESTADO_CANCHA:
  Aprobado     = "01"
  Pendiente    = "02"
  Rechazado    = "03"
  Suspendido   = "04"
  Mantenimiento = "05"

ESTADO_RESERVA:
  Pendiente    = "01"
  Confirmado   = "02"
  Cancelado    = "03"
  Expirado     = "04"

ESTADO_PAGO:
  Pagado       = "01"
  Parcial      = "02"
  Pendiente    = "03"
  Cancelado    = "04"
  Rechazado    = "05"

METODO_PAGO:
  Tarjeta      = "01"
  Efectivo     = "02"
  Transferencia = "03"
  Yape         = "04"
  Plin         = "05"

ESTADO_PROVEEDOR:
  Pendiente    = "01"
  Aprobado     = "02"
  Rechazado    = "03"

ESTADO_USUARIO:
  Activo       = "01"
  Inactivo     = "02"
  Suspendido   = "03"

Role:
  ADMIN, PROVEEDOR, CLIENTE, OPERADOR
```

---

## 4. PATRON DE RESPUESTA ESTANDAR

Todos los endpoints devuelven `ResponseDto<T>`:

```csharp
{
  "data": { ... },          // El objeto de respuesta
  "messages": [             // Mensajes del proceso
    {
      "message": "Cancha creada correctamente",
      "messageType": "Ok"   // Ok | Error | Warning | Info
    }
  ],
  "isValid": true           // false si hay mensajes de tipo Error
}
```

---

## 5. PATRON DE TRANSACCIONES (Unit of Work)

Todos los comandos (create, update, delete) estan envueltos automaticamente en una transaccion:

- **Commit automatico** si todo sale bien
- **Rollback automatico** si hay cualquier error
- **3 reintentos** ante errores de concurrencia
- **Audit trail automatico**: `UserNameCreate`, `CreateDate`, `UserNameUpdate`, `UpdateDate`, `Activo`

---

## 6. EJEMPLO COMPLETO: NEGOCIO DE CANCHA

A continuacion se muestra el flujo completo de **crear una cancha**, desde el endpoint hasta la base de datos.

### Entidad (`Reserva.Entity/Cancha.cs`)

Define la tabla en la base de datos:

```csharp
public class Cancha
{
    public int IdCancha { get; set; }
    .
    .
    .
    // Audit trail (gestionado automaticamente)
    public string UserNameCreate { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public string? UserNameUpdate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    public bool Activo { get; set; }

    // Navegacion (relaciones)
    public virtual Proveedor IdProveedorNavigation { get; set; }
    public virtual TipoSuperficie IdTipoSuperficieNavigation { get; set; }
}
```

---

### DTOs (`Reserva.Dto/Dbo/Cancha/`)

**`CreateCanchaDto.cs`** — lo que entra al crear:
```csharp
public class CreateCanchaDto
{
    public int IdProveedor { get; set; }
    .
    .
    .
    public List<int> IdsTipoDeportes { get; set; }               // Deportes disponibles
    public List<int> IdsServicios { get; set; }                  // Servicios (WiFi, estacionamiento, etc.)
}
```

**`GetCanchaDto.cs`** — lo que sale al obtener una cancha:
```csharp
public class GetCanchaDto : CanchaDto
{
    // Colecciones relacionadas
    public List<HorarioCanchaDto> HorariosCancha { get; set; }
    public List<ImagenCanchaDto> ImagenesCancha { get; set; }
    public List<TipoDeporteDto> TipoDeportes { get; set; }
    public List<ServicioDto> Servicios { get; set; }

    // Config del proveedor
    public int DuracionPreReserva { get; set; }
    public decimal PorcentajeAdelantoMinimo { get; set; }
}
```

---

### Command (`Reserva.Domain/Commands/Dbo/Cancha/`)

**Paso 1 — `CreateCanchaCommand.cs`**: Define el contrato

```csharp
public class CreateCanchaCommand : IRequest<ResponseDto<GetCanchaDto>>
{
    public CreateCanchaDto CreateDto { get; set; }
}
```

**Paso 2 — `CreateCanchaCommandValidator.cs`**: Valida los datos antes de ejecutar

```csharp
public class CreateCanchaCommandValidator : CommandValidatorBase<CreateCanchaCommand>
{
    public CreateCanchaCommandValidator(IRepository<Proveedor> proveedorRepository)
    {
        RequiredInformation(x => x.CreateDto).DependentRules(() =>
        {
            RuleFor(x => x.CreateDto.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido");

            RuleFor(x => x.CreateDto.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

            RuleFor(x => x.CreateDto.IdProveedor)
                .MustAsync(async (id, _) => await proveedorRepository.ExistsAsync(x => x.IdProveedor == id))
                .WithMessage("El proveedor no existe");
        });
    }
}
```

**Paso 3 — `CreateCanchaCommandHandler.cs`**: Ejecuta la logica de negocio

```csharp
public class CreateCanchaCommandHandler : IRequestHandler<CreateCanchaCommand, ResponseDto<GetCanchaDto>>
{
    private readonly IRepository<Cancha> _canchaRepository;
    private readonly IMapper _mapper;
    private readonly HorarioCanchaService _horarioService;

    public async Task<ResponseDto<GetCanchaDto>> Handle(CreateCanchaCommand request, CancellationToken ct)
    {
        var response = new ResponseDto<GetCanchaDto>();
        var dto = request.CreateDto;

        // 1. Generar codigo unico via stored procedure
        var codigo = await _canchaRepository.ExecuteScalarSPAsync<string>("sp_GenerarCodigoCancha");

        // 2. Mapear DTO a entidad
        var cancha = _mapper.Map<Cancha>(dto);
        cancha.Codigo = codigo;

        // 3. Estado inicial = Pendiente (requiere aprobacion del admin)
        cancha.IdEstadoCancha = ESTADO_CANCHA.Pendiente;

        // 4. Expandir horarios de 1 hora a bloques de 30 minutos
        //    Ej: 09:00-10:00 → [09:00-09:30, 09:30-10:00]
        cancha.HorarioCancha = _horarioService.ExpandirHorariosCreate(dto.HorarioCanchas);

        // 5. Asociar tipos de deporte (many-to-many)
        cancha.TipoDeporteCancha = dto.IdsTipoDeportes
            .Select(id => new TipoDeporteCancha { IdTipoDeporte = id })
            .ToList();

        // 6. Asociar servicios (many-to-many)
        cancha.ServicioCancha = dto.IdsServicios
            .Select(id => new ServicioCancha { IdServicio = id })
            .ToList();

        // 7. Guardar (el UnitOfWork hace commit automatico y agrega audit trail)
        await _canchaRepository.AddAsync(cancha);

        // 8. Mapear resultado y responder
        response.UpdateData(_mapper.Map<GetCanchaDto>(cancha));
        response.AddOkResult("Cancha creada correctamente");

        return response;
    }
}
```

---

### Query (`Reserva.Domain/Queries/Dbo/Cancha/`)

**`GetCanchaQueryHandler.cs`**: Obtiene una cancha con todas sus relaciones

```csharp
public class GetCanchaQueryHandler : IRequestHandler<GetCanchaQuery, ResponseDto<GetCanchaDto>>
{
    public async Task<ResponseDto<GetCanchaDto>> Handle(GetCanchaQuery request, CancellationToken ct)
    {
        var response = new ResponseDto<GetCanchaDto>();

        // Cargar cancha con todas sus relaciones
        var cancha = await _canchaRepository.GetByAsync(
            x => x.IdCancha == request.Id,
            x => x.ImagenCancha.Where(i => i.Activo),
            x => x.TipoDeporteCancha.Where(td => td.Activo),
            x => x.HorarioCancha,
            x => x.ServicioCancha,
            x => x.IdProveedorNavigation.ConfiguracionProveedor
        );

        if (cancha == null)
        {
            response.AddErrorResult("Cancha no encontrada");
            return response;
        }

        var canchaDto = _mapper.Map<GetCanchaDto>(cancha);

        // Comprimir horarios de 30 min de vuelta a rangos horarios para mostrar
        canchaDto.HorariosCancha = _horarioService.ComprimirHorarios(cancha.HorarioCancha.ToList());

        // Agregar config del proveedor
        var config = cancha.IdProveedorNavigation.ConfiguracionProveedor.FirstOrDefault();
        canchaDto.DuracionPreReserva = config?.DuracionPreReserva ?? 0;
        canchaDto.PorcentajeAdelantoMinimo = config?.PorcentajeAdelantoMinimo ?? 0;

        response.UpdateData(canchaDto);
        return response;
    }
}
```

---

### Controller (`Reserva.Api/Controllers/Dbo/CanchaController.cs`)

El controlador es solo un intermediario, implementa la interfaz de aplicacion y delega al servicio:

```csharp
[ApiController]
[Route("api/Cancha")]
[Security.Authorize]
public class CanchaController : ControllerBase, ICanchaApplication
{
    private readonly ICanchaApplication _CanchaApplication;

    public CanchaController(ICanchaApplication CanchaApplication)
    {
        _CanchaApplication = CanchaApplication;
    }

    [HttpPost]
    public async Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto)
        => await _CanchaApplication.Create(createDto);

    [HttpPut]
    public async Task<ResponseDto<GetCanchaDto>> Update(UpdateCanchaDto updateDto)
        => await _CanchaApplication.Update(updateDto);

    [HttpDelete("{id}")]
    public async Task<ResponseDto> Delete(int id)
        => await _CanchaApplication.Delete(id);

    [HttpGet("{id}")]
    public async Task<ResponseDto<GetCanchaDto>> Get(int id)
        => await _CanchaApplication.Get(id);

    [HttpPost("search")]
    public async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams)
        => await _CanchaApplication.Search(searchParams);
}
```

---

## 7. FLUJO COMPLETO DE UNA PETICION

```
HTTP Request POST /api/Cancha
        │
        ▼
CanchaController.Create()
  └── _mediator.Send(CreateCanchaCommand)
              │
              ▼
  CreateCanchaCommandValidator   ← FluentValidation
  (Si falla → ResponseDto con errores, no llega al Handler)
              │ (si pasa)
              ▼
  CreateCanchaCommandHandler     ← Logica de negocio
    ├── GenerarCodigo (SP)
    ├── Mapear DTO → Entity
    ├── Asignar estado Pendiente
    ├── Expandir horarios a 30min
    ├── Asociar deportes y servicios
    └── AddAsync(cancha)
              │
              ▼
  UnitOfWork.Commit()            ← Transaccion automatica
    ├── Audit trail automatico
    └── SaveChanges()
              │
              ▼
  ResponseDto<GetCanchaDto>      ← Respuesta estandar
        │
        ▼
HTTP Response 200 OK
{
  "data": { "idCancha": 1, "nombre": "Cancha Norte", ... },
  "messages": [{ "message": "Cancha creada correctamente", "messageType": "Ok" }],
  "isValid": true
}
```

---

## 8. LOGICA DE HORARIOS (HorarioCanchaService)

Este servicio es parte de la arquitectura transversal (fuera de `dbo/`) pero apoya el negocio:

**Expansion** (al crear/actualizar):
- Entrada: horario de 1 hora (ej: 09:00 a 11:00)
- Proceso: divide en bloques de 30 minutos
- Salida: [09:00-09:30, 09:30-10:00, 10:00-10:30, 10:30-11:00]
- **Proposito**: permite reservas en fracciones de 30 minutos

**Compresion** (al consultar):
- Proceso inverso: une bloques consecutivos del mismo dia
- Salida: horarios legibles para mostrar al usuario

---

## 9. ESTADO DE UNA CANCHA

```
[Creada] → Pendiente (02)
               │
      [Admin aprueba]
               │
               ▼
          Aprobado (01) ← disponible para reservas
               │
    ┌──────────┼──────────┐
    ▼          ▼          ▼
Suspendido  Mantenimiento Rechazado
  (04)         (05)         (03)
```

Solo las canchas en estado **Aprobado** aparecen en las busquedas publicas.

---

## 10. ENDPOINTS DISPONIBLES

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| POST | `/api/Cancha` | Crear cancha |
| PUT | `/api/Cancha` | Actualizar cancha |
| DELETE | `/api/Cancha/{id}` | Eliminar cancha (soft delete) |
| GET | `/api/Cancha/{id}` | Obtener cancha por ID |
| POST | `/api/Cancha/search` | Busqueda avanzada con filtros |
| GET | `/api/Cancha/list/{idProveedor}` | Listar canchas de un proveedor |
| GET | `/api/Cancha/selectcombo` | Para llenar combos/dropdowns |
| POST | `/api/Cancha/{id}/imagenes` | Subir imagenes |
| DELETE | `/api/Cancha/imagenes/{idImagen}` | Eliminar imagen |

---

## 11. ENTIDADES RELACIONADAS CON CANCHA

```
Cancha
  ├── Proveedor              ← Quien es dueno de la cancha
  │     └── ConfiguracionProveedor  ← Reglas del proveedor (duracion pre-reserva, adelanto minimo)
  │
  ├── TipoSuperficie         ← Cesped, cemento, sintetico, etc.
  ├── EstadoCancha           ← Pendiente, Aprobado, Rechazado, etc.
  ├── Ubigeo                 ← Codigo de ubicacion geografica (Peru)
  │
  ├── HorarioCancha[]        ← Horarios de atencion (por dia de la semana)
  ├── ImagenCancha[]         ← Fotos de la cancha
  ├── TipoDeporteCancha[]    ← Deportes que se pueden jugar
  │     └── TipoDeporte      ← Futbol, Voley, Basquet, etc.
  ├── ServicioCancha[]       ← Servicios incluidos
  │     └── Servicio         ← WiFi, Estacionamiento, Vestuarios, etc.
  ├── BloqueoHorario[]       ← Franjas bloqueadas por el proveedor
  ├── OperadorCancha[]       ← Personal asignado a la cancha
  ├── CanchaFavorita[]       ← Favoritos de los usuarios
  └── Reserva[]              ← Reservas realizadas en esta cancha
```

---

*Documentacion generada para el proyecto ReservaCanchas - Backend API*
