---
name: dotnet-application
description: Application Layer - Facade pattern, MediatR dispatch, and interface contracts for ReservaCanchas API
---

## 1. Estructura de una Application

### Interface (Application.Abstractions)

```csharp
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MiEntidad;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IMiEntidadApplication
    {
        Task<ResponseDto<GetMiEntidadDto>> Create(CreateMiEntidadDto createDto);
        Task<ResponseDto<GetMiEntidadDto>> Get(int id);
        Task<ResponseDto> Update(UpdateMiEntidadDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<SearchResultDto<GetMiEntidadDto>>> Search(SearchMiEntidadFilterDto filter, SearchParamsDto searchParams);
        Task<ResponseDto<List<SelectMiEntidadDto>>> Select();
        Task<ResponseDto<SelectComboMiEntidadDto>> SelectCombo();
    }
}
```

### Implementation (Application)

```csharp
using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MiEntidad;
using Reserva.Domain.Commands.Dbo.MiEntidad;
using Reserva.Domain.Queries.Dbo.MiEntidad;

namespace Reserva.Application.Dbo
{
    public class MiEntidadApplication : ApplicationBase, IMiEntidadApplication
    {
        public MiEntidadApplication(IMediator mediator) : base(mediator)
        {
        }

        public async Task<ResponseDto<GetMiEntidadDto>> Create(CreateMiEntidadDto createDto)
            => await _mediator.Send(new CreateMiEntidadCommand(createDto));

        public async Task<ResponseDto<GetMiEntidadDto>> Get(int id)
            => await _mediator.Send(new GetMiEntidadQuery(id));

        public async Task<ResponseDto> Update(UpdateMiEntidadDto updateDto)
            => await _mediator.Send(new UpdateMiEntidadCommand(updateDto));

        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteMiEntidadCommand(id));

        public async Task<ResponseDto<SearchResultDto<GetMiEntidadDto>>> Search(
            SearchMiEntidadFilterDto filter, SearchParamsDto searchParams)
            => await _mediator.Send(new SearchMiEntidadQuery(filter, searchParams));

        public async Task<ResponseDto<List<SelectMiEntidadDto>>> Select()
            => await _mediator.Send(new SelectMiEntidadQuery());

        public async Task<ResponseDto<SelectComboMiEntidadDto>> SelectCombo()
            => await _mediator.Send(new SelectComboMiEntidadQuery());
    }
}
```

---

## 2. Convenciones de Nombres

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Interface | `I[Entity]Application.cs` | `IMiEntidadApplication.cs` |
| Implementation | `[Entity]Application.cs` | `MiEntidadApplication.cs` |
| Interface Folder | `Reserva.Application.Abstractions/Dbo/` | `Reserva.Application.Abstractions/Dbo/IMiEntidadApplication.cs` |
| Implementation Folder | `Reserva.Application/Dbo/` | `Reserva.Application/Dbo/MiEntidadApplication.cs` |

---

## 3. Métodos Estándar CRUD

| Método | Command/Query | Retorno |
|--------|---------------|---------|
| `Create(dto)` | `CreateMiEntidadCommand` | `ResponseDto<GetMiEntidadDto>` |
| `Get(id)` | `GetMiEntidadQuery` | `ResponseDto<GetMiEntidadDto>` |
| `Update(dto)` | `UpdateMiEntidadCommand` | `ResponseDto` |
| `Delete(id)` | `DeleteMiEntidadCommand` | `ResponseDto` |
| `Search(filter, searchParams)` | `SearchMiEntidadQuery` | `ResponseDto<SearchResultDto<GetMiEntidadDto>>` |
| `Select()` | `SelectMiEntidadQuery` | `ResponseDto<List<SelectMiEntidadDto>>` |
| `SelectCombo()` | `SelectComboMiEntidadQuery` | `ResponseDto<SelectComboMiEntidadDto>>` |

---

## 4. Application Base Class

```csharp
using MediatR;

namespace Reserva.Application.Base
{
    public abstract class ApplicationBase
    {
        protected readonly IMediator _mediator;

        protected ApplicationBase(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
```

---

## 5. Registro de Dependencias

Las Application classes se registran automáticamente via Scrutor:

```csharp
// En Program.cs o Startup.cs
services.Scan(selector => selector
    .FromAssemblies(assembly)
    .AddClasses(x => x.Where(c => c.Name.EndsWith("Application")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);
```

**NO registrar manualmente** en `Program.cs`.

---

## 6. Patrón Facade

La Application Layer es un **Facade/Proxy** que:
- Recibe llamadas del Controller
- Despacha a MediatR (Commands o Queries)
- Retorna la respuesta sin lógica de negocio

```csharp
// Ejemplo simplificado
public async Task<ResponseDto<GetMiEntidadDto>> Create(CreateMiEntidadDto createDto)
    => await _mediator.Send(new CreateMiEntidadCommand(createDto));
```

---

## 7. Más Info

- Para Commands → skill("dotnet-command")
- Para Queries → skill("dotnet-query")
- Para Entities/DTOs → skill("dotnet-entity")
- Para Controllers → skill("dotnet-controller")
