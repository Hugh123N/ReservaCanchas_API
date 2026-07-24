---
name: dotnet-validator
description: FluentValidation - Command and Query validators for ReservaCanchas API
---

## 1. Estructura de un Command Validator

### Validator para Create (con validación de existencia)

```csharp
using FluentValidation;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions;
using Reserva.Entity.Entities;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class CreateMiEntidadCommandValidator : CommandValidatorBase<CreateMiEntidadCommand>
    {
        private readonly IRepository<Categoria> _categoriaRepository;

        public CreateMiEntidadCommandValidator(IRepository<Categoria> categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;

            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RuleFor(x => x.CreateDto.Nombre)
                    .NotEmpty()
                    .WithMessage("El nombre es requerido")
                    .MaximumLength(100)
                    .WithMessage("El nombre no puede exceder 100 caracteres");

                RuleFor(x => x.CreateDto.IdCategoria)
                    .MustAsync(ValidateCategoriaExistence)
                    .WithMessage("La categoría no existe");
            });
        }

        private async Task<bool> ValidateCategoriaExistence(
            CreateMiEntidadCommand command,
            int id,
            ValidationContext<CreateMiEntidadCommand> context,
            CancellationToken cancellationToken)
        {
            var exists = await _categoriaRepository
                .FindAll()
                .Where(x => x.IdCategoria == id && x.Activo)
                .AnyAsync(cancellationToken);

            return exists;
        }
    }
}
```

### Validator para Delete (simple)

```csharp
using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class DeleteMiEntidadCommandValidator : CommandValidatorBase<DeleteMiEntidadCommand>
    {
        public DeleteMiEntidadCommandValidator()
        {
            RuleFor(x => x.IdMiEntidad)
                .GreaterThan(0)
                .WithMessage("El ID es requerido");
        }
    }
}
```

---

## 2. Estructura de un Query Validator

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

## 3. Métodos de Validación Disponibles

### En CommandValidatorBase

| Método | Uso | Ejemplo |
|--------|-----|---------|
| `RequiredInformation()` | Validar información requerida | `RequiredInformation(x => x.CreateDto)` |
| `RequiredField()` | Validar campo requerido | `RequiredField(x => x.Id)` |
| `RequiredString()` | Validar string requerido | `RequiredString(x => x.Nombre)` |
| `MinimumLength()` | Longitud mínima | `MinimumLength(x => x.Nombre, 3)` |
| `MaximumLength()` | Longitud máxima | `MaximumLength(x => x.Nombre, 100)` |
| `GreaterThan()` | Mayor que | `GreaterThan(x => x.Monto, 0)` |
| `GreaterThanOrEqualTo()` | Mayor o igual que | `GreaterThanOrEqualTo(x => x.Monto, 1)` |
| `LessThan()` | Menor que | `LessThan(x => x.Monto, 1000)` |
| `InclusiveBetween()` | Entre (inclusivo) | `InclusiveBetween(x => x.Monto, 1, 1000)` |
| `MustAsync()` | Validación asíncrona | `MustAsync(ValidateExistenceAsync)` |

### En QueryValidatorBase

| Método | Uso | Ejemplo |
|--------|-----|---------|
| `RequiredField()` | Validar campo requerido | `RequiredField(x => x.Id)` |

---

## 4. Validación de Existencia (con Repository)

```csharp
// Patrón estándar
RuleFor(x => x.CreateDto.IdCategoria)
    .MustAsync(ValidateCategoriaExistence)
    .WithMessage("La categoría no existe");

private async Task<bool> ValidateCategoriaExistence(
    CreateMiEntidadCommand command,
    int id,
    ValidationContext<CreateMiEntidadCommand> context,
    CancellationToken cancellationToken)
{
    var exists = await _categoriaRepository
        .FindAll()
        .Where(x => x.IdCategoria == id && x.Activo)
        .AnyAsync(cancellationToken);

    return exists;
}
```

---

## 5. Reglas Dependientes

```csharp
RequiredInformation(x => x.CreateDto).DependentRules(() =>
{
    // Estas reglas solo se ejecutan si CreateDto no es null
    RuleFor(x => x.CreateDto.Nombre)
        .NotEmpty()
        .WithMessage("El nombre es requerido");

    RuleFor(x => x.CreateDto.IdCategoria)
        .MustAsync(ValidateCategoriaExistence)
        .WithMessage("La categoría no existe");
});
```

---

## 6. Mensajes de Error Personalizados

### Usando Resources (localización)

```csharp
RuleFor(x => x.IdMiEntidad)
    .NotEmpty()
    .WithMessage(Resources.Common.GetRecordNotFound);
```

### Mensaje personalizado

```csharp
RuleFor(x => x.CreateDto.Nombre)
    .NotEmpty()
    .WithMessage("El nombre del producto es obligatorio")
    .MaximumLength(100)
    .WithMessage("El nombre no puede exceder 100 caracteres");
```

---

## 7. Convenciones de Nombres

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Command Validator | `[Acción][Entity]CommandValidator.cs` | `CreateMiEntidadCommandValidator.cs` |
| Query Validator | `Get[Entity]QueryValidator.cs` | `GetMiEntidadQueryValidator.cs` |
| Folder Command | `Reserva.Domain/Commands/Dbo/[Entity]/` | `Reserva.Domain/Commands/Dbo/MiEntidad/` |
| Folder Query | `Reserva.Domain/Queries/Dbo/[Entity]/` | `Reserva.Domain/Queries/Dbo/MiEntidad/` |

---

## 8. Registro de Dependencias

Los validators se registran automáticamente via Scrutor:

```csharp
services.Scan(selector => selector
    .FromAssemblies(assembly)
    .AddClasses(x => x.Where(c => c.Name.EndsWith("Validator")))
    .AsSelf()
    .WithScopedLifetime()
);
```

**NO registrar manualmente** en `Program.cs`.

---

## 9. Errores Comunes

### ❌ No crear validator manualmente para Search queries
El `SearchQueryValidatorBase` se aplica automáticamente.

### ❌ No usar constructor sin parámetros si se necesita Repository
Si el validator necesita acceder a datos, inyectar el Repository.

### ❌ No olvidar DependentRules
Si el DTO es nullable, usar `DependentRules` para evitar errores.

---

## 10. Más Info

- Para Commands → skill("dotnet-command")
- Para Queries → skill("dotnet-query")
- Para Entities/DTOs → skill("dotnet-entity")
- Para Application Layer → skill("dotnet-application")
- Para Controllers → skill("dotnet-controller")
