---
name: dotnet-query
description: Queries, Handlers, Validators - CQRS query patterns for ReservaCanchas API
---

## 1. Estructura de un Query

### Query Get (obtener por ID)

```csharp
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.MiEntidad
{
    public class GetMiEntidadQuery : QueryBase<GetMiEntidadDto>
    {
        public int IdMiEntidad { get; }

        public GetMiEntidadQuery(int id)
        {
            IdMiEntidad = id;
        }
    }
}
```

### Query Search (búsqueda paginada)

```csharp
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.MiEntidad;

namespace Reserva.Domain.Queries.Dbo.MiEntidad
{
    public class SearchMiEntidadQuery : SearchQueryBase<SearchMiEntidadFilterDto, GetMiEntidadDto>
    {
        public SearchMiEntidadQuery(SearchMiEntidadFilterDto filter, SearchParamsDto searchParams)
            : base(filter, searchParams)
        {
        }
    }
}
```

---

## 2. Estructura de un Handler

### Handler Get (obtener por ID)

```csharp
using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MiEntidad;
using Reserva.Entity.Entities;
using Reserva.Repository.Abstractions;

namespace Reserva.Domain.Queries.Dbo.MiEntidad
{
    public class GetMiEntidadQueryHandler : QueryHandlerBase<GetMiEntidadQuery, GetMiEntidadDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<MiEntidad> _miEntidadRepository;

        public GetMiEntidadQueryHandler(
            IMapper mapper,
            IMediator mediator,
            GetMiEntidadQueryValidator validator,
            IRepository<MiEntidad> miEntidadRepository)
            : base(mediator, validator)
        {
            _mapper = mapper;
            _miEntidadRepository = miEntidadRepository;
        }

        protected override async Task<ResponseDto<GetMiEntidadDto>> HandleQuery(
            GetMiEntidadQuery request, 
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetMiEntidadDto>();

            // 1. Buscar la entidad con navegaciones
            var entidad = await _miEntidadRepository.GetByAsNoTrackingAsync(
                x => x.IdMiEntidad == request.IdMiEntidad,
                x => x.IdCategoriaNavigation, // Incluir navegación
                cancellationToken: cancellationToken);

            if (entidad == null)
            {
                response.AddErrorResult("MiEntidad no encontrada");
                return response;
            }

            // 2. Mapear a DTO
            var resultado = _mapper.Map<GetMiEntidadDto>(entidad);
            
            // 3. Enriquecer con datos de navegación (si es necesario)
            resultado.NombreCategoria = entidad.IdCategoriaNavigation?.Nombre;

            response.UpdateData(resultado);
            return response;
        }
    }
}
```

### Handler Search (búsqueda paginada)

```csharp
using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MiEntidad;
using Reserva.Entity.Entities;
using Reserva.Repository.Abstractions;

namespace Reserva.Domain.Queries.Dbo.MiEntidad
{
    public class SearchMiEntidadQueryHandler : SearchQueryHandlerBase<SearchMiEntidadQuery, SearchMiEntidadFilterDto, GetMiEntidadDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<MiEntidad> _miEntidadRepository;

        public SearchMiEntidadQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<MiEntidad> miEntidadRepository)
            : base(mediator, mapper)
        {
            _mapper = mapper;
            _miEntidadRepository = miEntidadRepository;
        }

        protected override async Task<SearchResultDto<GetMiEntidadDto>> HandleQuery(
            SearchMiEntidadQuery request, 
            CancellationToken cancellationToken)
        {
            // 1. Construir filtros dinámicos
            var filter = request.Filter;
            var searchExpression = filter.And(x => x.Activo);

            if (!string.IsNullOrEmpty(filter.Nombre))
            {
                searchExpression = searchExpression.And(x => x.Nombre.Contains(filter.Nombre));
            }

            if (filter.IdCategoria.HasValue)
            {
                searchExpression = searchExpression.And(x => x.IdCategoria == filter.IdCategoria.Value);
            }

            // 2. Ejecutar búsqueda paginada
            var result = await _miEntidadRepository.SearchByAsNoTrackingAsync(
                request.SearchParams.Page,
                request.SearchParams.PageSize,
                request.SearchParams.Sort,
                searchExpression,
                cancellationToken);

            // 3. Mapear resultados
            var items = _mapper.Map<List<GetMiEntidadDto>>(result.Items);

            return new SearchResultDto<GetMiEntidadDto>
            {
                Items = items,
                Total = result.Total,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
```

---

## 3. Estructura de un Validator

### Validator para Get Query

```csharp
using FluentValidation;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.MiEntidad
{
    public class GetMiEntidadQueryValidator : QueryValidatorBase<GetMiEntidadQuery>
    {
        public GetMiEntidadQueryValidator()
        {
            RuleFor(x => x.IdMiEntidad)
                .GreaterThan(0)
                .WithMessage("El ID es requerido");
        }
    }
}
```

### Validator para Search Query (automático)

El `SearchQueryValidatorBase` se aplica automáticamente y valida:
- `Page > 0`
- `PageSize > 0`
- `Sort` direction válido (asc/desc)

**NO crear validator manualmente** para Search queries.

---

## 4. Convenciones de Nombres

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Query Get | `Get[Entity]Query.cs` | `GetMiEntidadQuery.cs` |
| Query Search | `Search[Entity]Query.cs` | `SearchMiEntidadQuery.cs` |
| Handler Get | `Get[Entity]QueryHandler.cs` | `GetMiEntidadQueryHandler.cs` |
| Handler Search | `Search[Entity]QueryHandler.cs` | `SearchMiEntidadQueryHandler.cs` |
| Validator Get | `Get[Entity]QueryValidator.cs` | `GetMiEntidadQueryValidator.cs` |
| Filter DTO | `Search[Entity]FilterDto.cs` | `SearchMiEntidadFilterDto.cs` |
| Query Folder | `Reserva.Domain/Queries/Dbo/[Entity]/` | `Reserva.Domain/Queries/Dbo/MiEntidad/` |

---

## 5. Métodos Disponibles en Repository

| Método | Uso | Descripción |
|--------|-----|-------------|
| `GetByAsNoTrackingAsync()` | Obtener una entidad | Con include de navegaciones |
| `FindByAsNoTrackingAsync()` | Obtener múltiples entidades | Con filtro |
| `SearchByAsNoTrackingAsync()` | Búsqueda paginada | Retorna SearchResultDto |
| `FindAll()` | IQueryable para LINQ | Para filtros dinámicos |

---

## 6. Construcción de Filtros Dinámicos

```csharp
// Método And() para combinar filtros
var searchExpression = filter.And(x => x.Activo);

// Filtros condicionales
if (!string.IsNullOrEmpty(filter.Nombre))
{
    searchExpression = searchExpression.And(x => x.Nombre.Contains(filter.Nombre));
}

if (filter.FechaInicio.HasValue)
{
    searchExpression = searchExpression.And(x => x.Fecha >= filter.FechaInicio.Value);
}
```

---

## 7. Diferencias con Commands

| Aspecto | Commands | Queries |
|---------|----------|---------|
| Transacción | Sí (automática) | No |
| Concurrency retry | Sí (3 intentos) | No |
| Modifica datos | Sí | No |
| Base Handler | `CommandHandlerBase` | `QueryHandlerBase` |
| Return type | `ResponseDto` o `ResponseDto<T>` | `ResponseDto<T>` o `SearchResultDto<T>` |

---

## 8. Más Info

- Para Commands → skill("dotnet-command")
- Para Entities/DTOs → skill("dotnet-entity")
- Para Application Layer → skill("dotnet-application")
- Para Controllers → skill("dotnet-controller")
- Para Validators → skill("dotnet-validator")
