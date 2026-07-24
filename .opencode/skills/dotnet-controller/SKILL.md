---
name: dotnet-controller
description: API Controllers - HTTP endpoints, routing, and pass-through pattern for ReservaCanchas API
---

## 1. Estructura de un Controller

### Controller Base (CRUD estándar)

```csharp
using Microsoft.AspNetCore.Mvc;
using Reserva.Api.Controllers.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MiEntidad;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiEntidadController : ApiControllerBase, IMiEntidadApplication
    {
        private readonly IMiEntidadApplication _miEntidadApplication;

        public MiEntidadController(IMiEntidadApplication miEntidadApplication)
        {
            _miEntidadApplication = miEntidadApplication;
        }

        [HttpPost]
        public async Task<ResponseDto<GetMiEntidadDto>> Create([FromBody] CreateMiEntidadDto createDto)
            => await _miEntidadApplication.Create(createDto);

        [HttpGet("{id}")]
        public async Task<ResponseDto<GetMiEntidadDto>> Get(int id)
            => await _miEntidadApplication.Get(id);

        [HttpPut]
        public async Task<ResponseDto> Update([FromBody] UpdateMiEntidadDto updateDto)
            => await _miEntidadApplication.Update(updateDto);

        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _miEntidadApplication.Delete(id);

        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<GetMiEntidadDto>>> Search(
            [FromBody] SearchMiEntidadFilterDto filter,
            [FromQuery] SearchParamsDto searchParams)
            => await _miEntidadApplication.Search(filter, searchParams);

        [HttpGet("select")]
        public async Task<ResponseDto<List<SelectMiEntidadDto>>> Select()
            => await _miEntidadApplication.Select();

        [HttpGet("selectcombo")]
        public async Task<ResponseDto<SelectComboMiEntidadDto>> SelectCombo()
            => await _miEntidadApplication.SelectCombo();
    }
}
```

---

## 2. Convenciones de Routing

| HTTP Verb | Ruta | Método | Ejemplo |
|-----------|------|--------|---------|
| `[HttpPost]` | `/api/{Entity}` | Create | `POST /api/MiEntidad` |
| `[HttpGet("{id}")]` | `/api/{Entity}/{id}` | Get | `GET /api/MiEntidad/1` |
| `[HttpPut]` | `/api/{Entity}` | Update | `PUT /api/MiEntidad` |
| `[HttpDelete("{id}")]` | `/api/{Entity}/{id}` | Delete | `DELETE /api/MiEntidad/1` |
| `[HttpPost("search")]` | `/api/{Entity}/search` | Search | `POST /api/MiEntidad/search` |
| `[HttpGet("select")]` | `/api/{Entity}/select` | Select | `GET /api/MiEntidad/select` |
| `[HttpGet("selectcombo")]` | `/api/{Entity}/selectcombo` | SelectCombo | `GET /api/MiEntidad/selectcombo` |
| `[HttpPost("list")]` | `/api/{Entity}/list` | List | `POST /api/MiEntidad/list` |

---

## 3. Endpoints Personalizados

```csharp
[HttpPost("custom-action")]
public async Task<ResponseDto<CustomDto>> CustomAction([FromBody] CustomRequestDto request)
    => await _miEntidadApplication.CustomAction(request);

[HttpGet("get-by-date")]
public async Task<ResponseDto<List<GetMiEntidadDto>>> GetByDate(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    => await _miEntidadApplication.GetByDate(startDate, endDate);
```

---

## 4. Controller Base Class

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reserva.Api.Controllers.Base
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IConfiguration Configuration { get; }
        protected IWebHostEnvironment Environment { get; }

        protected ApiControllerBase()
        {
            // Resueltos via DI en tiempo de ejecución
        }
    }
}
```

---

## 5. Patrón Pass-Through

Los Controllers son **puros delegados**:
- Zero lógica de negocio
- Solo desvían a la Application Layer
- La Application Layer desvía a MediatR

```
Controller → Application → MediatR → Handler → Repository → Entity
```

---

## 6. Convenciones de Nombres

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Controller | `[Entity]Controller.cs` | `MiEntidadController.cs` |
| Folder | `Reserva.Api/Controllers/Dbo/` | `Reserva.Api/Controllers/Dbo/MiEntidadController.cs` |
| Ruta | `api/[Entity]` | `api/MiEntidad` |

---

## 7. Seguridad

### Autorización

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MiEntidadController : ApiControllerBase
{
    // Solo usuarios autenticados
}

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ApiControllerBase
{
    // Solo usuarios con rol Admin
}
```

### Claims de Usuario

```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userName = User.FindFirst(ClaimTypes.Name)?.Value;
```

---

## 8. Más Info

- Para Commands → skill("dotnet-command")
- Para Queries → skill("dotnet-query")
- Para Entities/DTOs → skill("dotnet-entity")
- Para Application Layer → skill("dotnet-application")
