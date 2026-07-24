---
name: dotnet-entity
description: Entities, DTOs, Models - Data models and transfer objects for ReservaCanchas API
---

## 1. Estructura de una Entity

### Entity Base

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva.Entity.Entities
{
    [Table("MiEntidad", Schema = "dbo")]
    public partial class MiEntidad
    {
        [Key]
        public int IdMiEntidad { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public int IdCategoria { get; set; }

        public bool Activo { get; set; }

        // Auditoría
        public string UserNameCreate { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string? UserNameUpdate { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        // Navegaciones
        [ForeignKey("IdCategoria")]
        public virtual Categoria IdCategoriaNavigation { get; set; }

        public virtual ICollection<MiEntidadDetalle> MiEntidadDetalle { get; set; } = new List<MiEntidadDetalle>();
    }
}
```

---

## 2. Estructura de DTOs

### Base DTO (compartido)

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class MiEntidadDto
    {
        public int IdMiEntidad { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public bool Activo { get; set; }
    }
}
```

### Create DTO

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class CreateMiEntidadDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int IdCategoria { get; set; }
    }
}
```

### Get DTO (con datos de navegación)

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class GetMiEntidadDto : MiEntidadDto
    {
        // Datos de navegación
        public string NombreCategoria { get; set; }

        // Auditoría
        public string UserNameCreate { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string? UserNameUpdate { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        // Colecciones
        public List<MiEntidadDetalleDto> Detalles { get; set; }
    }
}
```

### Update DTO

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class UpdateMiEntidadDto : MiEntidadDto
    {
        // Mismos campos que Create + Id
    }
}
```

### Search Filter DTO

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class SearchMiEntidadFilterDto
    {
        public string? Nombre { get; set; }
        public int? IdCategoria { get; set; }
        public bool? Activo { get; set; }
    }
}
```

### List DTO (para listados)

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class ListMiEntidadDto
    {
        public int IdMiEntidad { get; set; }
        public string Nombre { get; set; }
        public string NombreCategoria { get; set; }
        public bool Activo { get; set; }
    }
}
```

### Select DTO (para combos)

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class SelectMiEntidadDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}
```

### SelectCombo DTO (para select multi-opción)

```csharp
namespace Reserva.Dto.Dbo.MiEntidad
{
    public class SelectComboMiEntidadDto
    {
        public List<SelectMiEntidadDto> Categorias { get; set; }
        public List<SelectMiEntidadDto> Estados { get; set; }
    }
}
```

---

## 3. Convenciones de Nombres

### Entities

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Entity | `PascalCase` singular | `MiEntidad.cs` |
| ID Principal | `Id[Entity]` | `IdMiEntidad` |
| ID Foránea | `Id[ReferencedEntity]` | `IdCategoria` |
| Navegación | `Id[Entity]Navigation` | `IdCategoriaNavigation` |
| Colección | `Plural` | `MiEntidadDetalle` |
| Soft Delete | `bool Activo` | `Activo` |
| Folder | `Reserva.Entity/` | `Reserva.Entity/MiEntidad.cs` |

### DTOs

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
| Folder | `Reserva.Dto/Dbo/[Entity]/` | `Reserva.Dto/Dbo/MiEntidad/` |

---

## 4. Patrones de Auditoría

Si una entidad tiene estas propiedades, se auditan automáticamente:

```csharp
public string UserNameCreate { get; set; }
public DateTimeOffset CreateDate { get; set; }
public string? UserNameUpdate { get; set; }
public DateTimeOffset? UpdateDate { get; set; }
public bool Activo { get; set; }
```

**NO setearlas manualmente** - el UnitOfWork las actualiza automáticamente.

---

## 5. Soft Delete Pattern

```csharp
// Entity
public bool Activo { get; set; }

// En Handler
entidad.Activo = false;
await _repository.UpdateAsync(entidad);
```

---

## 6. Mapeo con AutoMapper

### Profile de AutoMapper

```csharp
using AutoMapper;
using Reserva.Dto.Dbo.MiEntidad;
using Reserva.Entity.Entities;

namespace Reserva.Application.Mappings
{
    public class MiEntidadProfile : Profile
    {
        public MiEntidadProfile()
        {
            // Entity -> GetDto
            CreateMap<MiEntidad, GetMiEntidadDto>()
                .ForMember(dest => dest.NombreCategoria, 
                    opt => opt.MapFrom(src => src.IdCategoriaNavigation.Nombre));

            // CreateDto -> Entity
            CreateMap<CreateMiEntidadDto, MiEntidad>();

            // Entity -> ListDto
            CreateMap<MiEntidad, ListMiEntidadDto>()
                .ForMember(dest => dest.NombreCategoria, 
                    opt => opt.MapFrom(src => src.IdCategoriaNavigation.Nombre));

            // Entity -> SelectDto
            CreateMap<MiEntidad, SelectMiEntidadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdMiEntidad))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Nombre));
        }
    }
}
```

---

## 7. Respuesta Estándar

### ResponseDto Base

```csharp
public class ResponseDto
{
    public bool IsValid { get; set; }
    public List<ApplicationMessageDto> Messages { get; set; }

    public void AddOkResult(string message) { ... }
    public void AddErrorResult(string message) { ... }
    public void AddWarningResult(string message) { ... }
    public void AddInfoResult(string message) { ... }
}

public class ResponseDto<T> : ResponseDto
{
    public T Data { get; set; }

    public void UpdateData(T data) { ... }
}
```

### SearchResultDto (para búsquedas paginadas)

```csharp
public class SearchResultDto<T>
{
    public List<T> Items { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

---

## 8. Más Info

- Para Commands → skill("dotnet-command")
- Para Queries → skill("dotnet-query")
- Para Application Layer → skill("dotnet-application")
- Para Controllers → skill("dotnet-controller")
