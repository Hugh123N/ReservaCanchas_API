# Plan de Implementación - Control de Límites por Plan

## 1. Objetivo

Implementar validación de límites de uso según el plan contratado por el proveedor:
- **MAX_CANCHAS**: Limitar cantidad de canchas activas
- **MAX_OPERADORES**: Limitar cantidad de operadores activos

## 2. Arquitectura

### 2.1 Fuentes de Datos

| Fuente | Descripción |
|--------|-------------|
| `ProveedorPlan` WHERE `EsActual=1` | Plan actual del proveedor |
| `PlanLimite` WHERE `Codigo IN ('MAX_CANCHAS','MAX_OPERADORES')` | Límites del plan |
| `Cancha` WHERE `IdProveedor=@id AND Activo=1` | Canchas activas (count) |
| `Operador` WHERE `IdProveedor=@id AND Activo=1` | Operadores activos (count) |

### 2.2 Estrategia: Contar desde tablas fuente

No se usa la tabla `UsoPlan` para tracking. El conteo se realiza directamente:
- `COUNT(*) Cancha WHERE IdProveedor=@id AND Activo=1`
- `COUNT(*) Operador WHERE IdProveedor=@id AND Activo=1`

### 2.3 Capas de Validación

```
FRONTEND (UI)                          BACKEND (API)
─────────────                          ─────────────
1. Consultar plan + limites             4. FluentValidation rechaza
2. Contar canchas/operadores            5. Handler valida antes de crear
3. Ocultar botón / Mostrar mensaje
```

## 3. Límites por Plan

| Plan | MAX_CANCHAS | MAX_OPERADORES |
|------|-------------|----------------|
| PRUEBA (FREE) | 1 | 1 |
| BASICO | 1 | 1 |
| PROFESIONAL | 3 | 5 |
| EMPRESARIAL | 999 | 999 (ilimitado) |

## 4. Endpoints Existentes (sin cambios necesarios)

| Endpoint | Propósito | Uso |
|----------|-----------|-----|
| `GET /api/ProveedorPlan/current/{idProveedor}` | Plan actual + limites[] | Frontend obtiene límites |
| `POST /api/Cancha/list {id}` | Lista canchas del proveedor | Frontend cuenta canchas |
| `POST /api/Operador/list {id}` | Lista operadores del proveedor | Frontend cuenta operadores |

## 5. Implementación Backend (ReservaCanchas_API)

### 5.1 Nuevo Servicio: PlanLimitValidationService

**Archivo**: `Reserva.Domain/Services/PlanLimitValidationService.cs`

```csharp
public class PlanLimitValidationService
{
    // Verifica si el proveedor puede crear más canchas
    Task<bool> CanCreateCanchaAsync(int idProveedor);

    // Verifica si el proveedor puede crear más operadores
    Task<bool> CanCreateOperadorAsync(int idProveedor);

    // Obtiene el límite de un código específico
    Task<int?> GetLimiteAsync(int idProveedor, string codigoLimite);

    // Obtiene el conteo actual de un recurso
    Task<int> GetCountAsync(int idProveedor, string recurso);
}
```

### 5.2 Archivos a Modificar

| Archivo | Cambio |
|---------|--------|
| `CreateCanchaCommandValidator.cs` | Agregar validación async de límite |
| `CreateCanchaCommandHandler.cs` | Agregar validación defensiva |
| `CreateOperadorCommandValidator.cs` | Agregar validación async de límite |
| `CreateOperadorCommandHandler.cs` | Agregar validación defensiva |

### 5.3 Flujo de Validación Backend

```
CreateCancha request
  │
  ├─ Validator:
  │   1. Obtener ProveedorPlan actual del proveedor
  │   2. Obtener PlanLimite WHERE Codigo='MAX_CANCHAS'
  │   3. Contar Canchas WHERE IdProveedor=@id AND Activo=1
  │   4. Si count >= limite → Error: "Ha alcanzado el límite de X canchas"
  │
  └─ Handler (doble verificación):
      1. Repetir validación por seguridad
      2. Si OK → crear cancha
```

## 6. Implementación Frontend (ReservaCanchas_admin_Web)

### 6.1 Nuevo Servicio: PlanLimitService

**Archivo**: `src/app/features/planes/core/services/plan-limit.service.ts`

Servicio que encapsula la lógica de verificación de límites:

```typescript
export class PlanLimitService {
  // Verifica si puede crear más canchas
  canCreateCancha(idProveedor: number): Observable<boolean>;

  // Verifica si puede crear más operadores
  canCreateOperador(idProveedor: number): Observable<boolean>;

  // Obtiene info completa de límites
  getLimitsInfo(idProveedor: number): Observable<LimitsInfo>;
}
```

### 6.2 Archivos a Modificar

| Archivo | Cambio |
|---------|--------|
| `canchas.component.ts` | Verificar límite al cargar lista |
| `canchas.component.html` | Botón deshabilitado + mensaje de límite |
| `operadores.component.ts` | Verificar límite al cargar lista |
| `operadores.component.html` | Botón deshabilitado + mensaje de límite |
| `crear-editar-cancha.component.ts` | Verificar límite antes de navegar |
| `crear-editar-operador.component.ts` | Verificar límite antes de navegar |

### 6.3 Flujo Frontend

```
Página Lista Canchas
  │
  ├─ ngOnInit():
  │   1. authService.getUserClaims()?.UserIdNegocio → idProveedor
  │   2. proveedorPlanService.getCurrent(idProveedor) → plan + limites
  │   3. canchaService.list(idProveedor) → count canchas
  │   4. Comparar: puedeCrear = count < limite
  │
  └─ Template:
      <button [disabled]="!puedeCrearCancha">
        <span *ngIf="!puedeCrearCancha">Límite alcanzado</span>
        <span *ngIf="puedeCrearCancha">Nueva Cancha</span>
      </button>
      <p *ngIf="!puedeCrearCancha" class="text-amber-600">
        Ha alcanzado el límite de {{limiteCanchas}} canchas de su plan {{nombrePlan}}.
        <a routerLink="/admin/planes">Mejorar plan</a>
      </p>
```

## 7. Resumen de Archivos

### Backend (ReservaCanchas_API)

| # | Archivo | Acción |
|---|---------|--------|
| 1 | `Reserva.Domain/Services/PlanLimitValidationService.cs` | **NUEVO** - Servicio de validación |
| 2 | `Reserva.Domain/Commands/Dbo/Cancha/CreateCanchaCommandValidator.cs` | **MODIFICAR** - Agregar validación |
| 3 | `Reserva.Domain/Commands/Dbo/Cancha/CreateCanchaCommandHandler.cs` | **MODIFICAR** - Validación defensiva |
| 4 | `Reserva.Domain/Commands/Dbo/Operador/CreateOperadorCommandValidator.cs` | **MODIFICAR** - Agregar validación |
| 5 | `Reserva.Domain/Commands/Dbo/Operador/CreateOperadorCommandHandler.cs` | **MODIFICAR** - Validación defensiva |

### Frontend (ReservaCanchas_admin_Web)

| # | Archivo | Acción |
|---|---------|--------|
| 1 | `src/app/features/planes/core/services/plan-limit.service.ts` | **NUEVO** - Servicio de límites |
| 2 | `src/app/features/planes/core/models/limits-info.model.ts` | **NUEVO** - Modelo de respuesta |
| 3 | `src/app/features/canchas/pages/canchas/canchas.component.ts` | **MODIFICAR** - Agregar verificación |
| 4 | `src/app/features/canchas/pages/canchas/canchas.component.html` | **MODIFICAR** - Botón + mensaje |
| 5 | `src/app/features/operadores/pages/operadores/operadores.component.ts` | **MODIFICAR** - Agregar verificación |
| 6 | `src/app/features/operadores/pages/operadores/operadores.component.html` | **MODIFICAR** - Botón + mensaje |

## 8. Mensajes de Error

| Codigo | Mensaje |
|--------|---------|
| `LIMITE_CANCHAS` | "Ha alcanzado el límite de {N} canchas de su plan {Plan}. Mejore su plan para crear más canchas." |
| `LIMITE_OPERADORES` | "Ha alcanzado el límite de {N} operadores de su plan {Plan}. Mejore su plan para crear más operadores." |

## 9. Notas de Implementación

- El frontend usa `authService.getUserClaims()?.UserIdNegocio` para obtener `idProveedor`
- El endpoint `GET /api/ProveedorPlan/current/{idProveedor}` ya retorna `limites[]` con `codigo` y `valor`
- El conteo se hace contando los registros activos en las tablas fuente
- La validación backend es defensa en profundidad (el frontend ya debe haber bloqueado)
- El link "Mejorar plan" lleva a `/admin/planes/catalogo`
