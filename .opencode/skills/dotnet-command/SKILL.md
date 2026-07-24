---
name: dotnet-command
description: Commands, Handlers, Validators - CQRS implementation patterns for ReservaCanchas API
---

## 1. Estructura de un Command

### Command Base (sin retorno de datos)

```csharp
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class DeleteMiEntidadCommand : CommandBase
    {
        public int IdMiEntidad { get; }

        public DeleteMiEntidadCommand(int id)
        {
            IdMiEntidad = id;
        }
    }
}
```

### Command Base (con retorno de datos)

```csharp
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.MiEntidad;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class CreateMiEntidadCommand : CommandBase<GetMiEntidadDto>
    {
        public CreateMiEntidadDto CreateDto { get; }

        public CreateMiEntidadCommand(CreateMiEntidadDto createDto)
        {
            CreateDto = createDto;
        }
    }
}
```

---

## 2. Estructura de un Handler

### Handler para Create/Update (con retorno de datos)

```csharp
using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.MiEntidad;
using Reserva.Entity.Entities;
using Reserva.Repository.Abstractions;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class CreateMiEntidadCommandHandler : CommandHandlerBase<CreateMiEntidadCommand, GetMiEntidadDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<MiEntidad> _miEntidadRepository;

        public CreateMiEntidadCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateMiEntidadCommandValidator validator,
            IRepository<MiEntidad> miEntidadRepository)
            : base(unitOfWork, mediator, validator)
        {
            _mapper = mapper;
            _miEntidadRepository = miEntidadRepository;
        }

        protected override async Task<ResponseDto<GetMiEntidadDto>> HandleCommand(
            CreateMiEntidadCommand request, 
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetMiEntidadDto>();

            // 1. Mapear DTO a entidad
            var entidad = _mapper.Map<MiEntidad>(request.CreateDto);

            // 2. Guardar en base de datos
            await _miEntidadRepository.AddAsync(entidad);
            await _unitOfWork.SaveAsync(cancellationToken);

            // 3. Mapear respuesta
            var resultado = _mapper.Map<GetMiEntidadDto>(entidad);
            response.UpdateData(resultado);
            response.AddOkResult("MiEntidad creada exitosamente");

            return response;
        }
    }
}
```

### Handler para Delete (sin retorno de datos)

```csharp
using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Entity.Entities;
using Reserva.Repository.Abstractions;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class DeleteMiEntidadCommandHandler : CommandHandlerBase<DeleteMiEntidadCommand>
    {
        private readonly IRepository<MiEntidad> _miEntidadRepository;

        public DeleteMiEntidadCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            DeleteMiEntidadCommandValidator validator,
            IRepository<MiEntidad> miEntidadRepository)
            : base(unitOfWork, mediator, validator)
        {
            _miEntidadRepository = miEntidadRepository;
        }

        protected override async Task<ResponseDto> HandleCommand(
            DeleteMiEntidadCommand request, 
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            // 1. Buscar la entidad
            var entidad = await _miEntidadRepository.GetByAsync(
                x => x.IdMiEntidad == request.IdMiEntidad,
                cancellationToken: cancellationToken);

            if (entidad == null)
            {
                response.AddErrorResult("MiEntidad no encontrada");
                return response;
            }

            // 2. Soft delete (marcar como inactivo)
            entidad.Activo = false;
            await _miEntidadRepository.UpdateAsync(entidad);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.AddOkResult("MiEntidad eliminada exitosamente");
            return response;
        }
    }
}
```

---

## 3. Estructura de un Validator

```csharpusing FluentValidation;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions;
using Reserva.Entity.Entities;

namespace Reserva.Domain.Commands.Dbo.MiEntidad
{
    public class CreateMiEntidadCommandValidator : CommandValidatorBase<CreateMiEntidadCommand>
    {
        private readonly IRepository<MiEntidad> _miEntidadRepository;

        public CreateMiEntidadCommandValidator(IRepository<MiEntidad> miEntidadRepository)
        {
            _miEntidadRepository = miEntidadRepository;

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
            var exists = await _miEntidadRepository
                .FindAll()
                .Where(x => x.IdCategoria == id)
                .AnyAsync(cancellationToken);

            return exists;
        }
    }
}
```

---

## 4. Convenciones de Nombres

| Elemento | Patrón | Ejemplo |
|----------|--------|---------|
| Command | `[Acción][Entity]Command.cs` | `CreateMiEntidadCommand.cs` |
| Handler | `[Acción][Entity]CommandHandler.cs` | `CreateMiEntidadCommandHandler.cs` |
| Validator | `[Acción][Entity]CommandValidator.cs` | `CreateMiEntidadCommandValidator.cs` |
| Command Folder | `Reserva.Domain/Commands/Dbo/[Entity]/` | `Reserva.Domain/Commands/Dbo/MiEntidad/` |

---

## 5. Métodos Disponibles en Handler Base

| Método | Uso | Descripción |
|--------|-----|-------------|
| `HandleCommand()` | Override obligatorio | Lógica principal del handler |
| `response.UpdateData(data)` | Establecer datos de respuesta | Asigna datos al ResponseDto |
| `response.AddOkResult(msg)` | Agregar mensaje de éxito | Mensaje de éxito |
| `response.AddErrorResult(msg)` | Agregar mensaje de error | Mensaje de error |

---

## 6. Inyección de Dependencias

Los handlers se registran automáticamente via Scrutor:
- Cualquier clase que implemente `IRequestHandler<TRequest, TResponse>` se registra como Scoped
- Los validators se registran como Scoped (cualquier clase que termine en "Validator")

**NO registrar manualmente** handlers o validators en `Program.cs`.

---

## 7. Transacciones

Los CommandHandlers ejecutan automáticamente en transacción:
- `UseTransaction => true` por defecto
- Commit automático si todo OK
- Rollback automático si hay error
- Reintentos automáticos en caso de `DbUpdateConcurrencyException` (3 intentos)

**NO crear transacciones manuales** a menos que sea absolutamente necesario.

---

## 8. Más Info

- Para Queries → skill("dotnet-query")
- Para Entities/DTOs → skill("dotnet-entity")
- Para Application Layer → skill("dotnet-application")
- Para Controllers → skill("dotnet-controller")
- Para Validators → skill("dotnet-validator")
