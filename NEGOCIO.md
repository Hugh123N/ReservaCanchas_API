# ReservaCanchas - Documentacion de Negocio

> Este documento describe toda la logica de negocio del sistema: modulos, entidades, flujos, reglas y endpoints.
> Para la documentacion de arquitectura tecnica ver `ARQUITECTURA.md`.

---

## INDICE

1. [Modelo de Negocio](#1-modelo-de-negocio)
2. [Roles y Permisos](#2-roles-y-permisos)
3. [Catalogos del Sistema](#3-catalogos-del-sistema)
4. [Modulo: Usuarios y Autenticacion](#4-modulo-usuarios-y-autenticacion)
5. [Modulo: Proveedor](#5-modulo-proveedor)
6. [Modulo: Cancha](#6-modulo-cancha)
7. [Modulo: Operador](#7-modulo-operador)
8. [Modulo: Calendario](#8-modulo-calendario)
9. [Modulo: Reserva](#9-modulo-reserva)
10. [Modulo: Pago](#10-modulo-pago)
11. [Modulo: Notificaciones](#11-modulo-notificaciones)
12. [Jobs en Background](#12-jobs-en-background)
13. [Flujos Completos de Negocio](#13-flujos-completos-de-negocio)
14. [Reglas de Negocio Globales](#14-reglas-de-negocio-globales)
15. [Pendientes de Implementacion](#15-pendientes-de-implementacion)

---

## 1. MODELO DE NEGOCIO

### Descripcion General

**ReservaCanchas** es una plataforma **B2B2C** para la reserva de canchas deportivas en Peru:

```
Plataforma ReservaCanchas
    │
    ├── PROVEEDOR (dueno de canchas)
    │     ├── Registra sus canchas
    │     ├── Configura horarios y precios
    │     ├── Contrata operadores
    │     └── Define politicas de cancelacion
    │
    ├── OPERADOR (empleado del proveedor)
    │     ├── Gestiona reservas del dia
    │     ├── Confirma pagos en efectivo
    │     └── Crea reservas manuales (telefono / presencial)
    │
    └── CLIENTE (usuario final)
          ├── Busca canchas disponibles
          ├── Reserva horarios
          └── Paga en efectivo al llegar
```

### Separacion Critica: Reservas vs Planes

| Concepto | Metodo de Pago | Confirmacion | Sistema |
|----------|----------------|--------------|---------|
| **Reservas de canchas** | EFECTIVO unicamente | Manual por operador | App cliente + panel admin |
| **Planes de proveedores** | Culqi (Yape/Plin/Tarjeta) | Automatica via webhook | Panel admin (futuro) |

> **IMPORTANTE**: Las reservas de canchas NO usan Culqi ni pagos online. El cliente paga en efectivo cuando llega a la cancha. El operador confirma el pago desde el panel.

---

## 2. ROLES Y PERMISOS

### Roles del Sistema

| Rol | Codigo | Descripcion |
|-----|--------|-------------|
| **Admin** | `Admin` | Superadministrador con acceso total |
| **Proveedor** | `Proveedor` | Dueno de canchas, gestiona su negocio |
| **Operador** | `Operador` | Empleado del proveedor, atiende reservas |
| **Cliente** | `Cliente` | Usuario final, reserva canchas |

### Matriz de Permisos por Modulo

| Modulo | Admin | Proveedor | Operador | Cliente |
|--------|-------|-----------|----------|---------|
| Canchas (CRUD) | SI | SI (propias) | NO | NO |
| Canchas (ver) | SI | SI | SI (asignadas) | SI |
| Operadores (CRUD) | SI | SI (propios) | NO | NO |
| Reservas (ver) | SI | SI (propias) | SI (canchas asignadas) | SI (propias) |
| Reservas (crear) | SI | SI | SI | SI |
| Reservas (confirmar) | SI | SI | SI | NO |
| Reservas (cancelar) | SI | SI | SI | SI |
| Calendario | SI | SI (todas sus canchas) | SI (canchas asignadas) | NO |
| Configuracion Proveedor | SI | SI (propia) | NO | NO |
| Dashboard Proveedor | SI | SI | NO | NO |
| Dashboard Operador | SI | SI | SI | NO |

### Claims del JWT

El token JWT contiene los siguientes claims personalizados:

```
UserId          → ID del usuario (GUID de AspNetUsers)
UserIdNegocio   → ID de negocio: IdProveedor si es Proveedor, IdOperador si es Operador
UserName        → Email del usuario
DisplayName     → Nombre completo
Telefono        → Telefono del usuario
Roles           → Array de roles asignados
```

---

## 3. CATALOGOS DEL SISTEMA

Tablas de referencia que raramente cambian. Todas tienen combo endpoint (`/selectcombo`).

### TipoDeporte

| Codigo | Nombre |
|--------|--------|
| FUT11 | Futbol 11 |
| FUT7 | Futbol 7 |
... mas

### TipoSuperficie

| Codigo | Nombre |
|--------|--------|
| GRASSNAT | Grass Natural |
... mas

### Servicio (amenidades de la cancha)

| Codigo | Nombre |
|--------|--------|
| ESTACION | Estacionamiento |
| VESTUA | Vestuarios |
... mas

### Hora

- Tabla con todos los bloques de 30 minutos del dia: `00:00`, `00:30`, `01:00`, ..., `23:30`
- Total: 48 registros
- Usada como FK en `HorarioCancha` y `BloqueoHorario`

### Ubigeo

Codigo geografico de Peru (formato: `DDPPDD` - departamento, provincia, distrito).
La base incluye los 43 distritos de Lima como datos de ejemplo.

---

## 4. MODULO: USUARIOS Y AUTENTICACION

### Entidad: AspNetUsers

Extiende la tabla estandar de ASP.NET Identity con campos adicionales:

```sql
Id              UNIQUEIDENTIFIER  -- PK (GUID auto-generado)
FirstName       VARCHAR(100)
LastName        VARCHAR(100)
Email           NVARCHAR(256)     -- UNIQUE, validado formato email
PhoneNumber     NVARCHAR(MAX)
imagen          VARCHAR(MAX)      -- URL de foto de perfil
idEstadoUsuario INT               -- Activo(01) | Inactivo(02) | Suspendido(03)
-- Campos de ASP.NET Identity: PasswordHash, SecurityStamp, etc.
-- Audit: userNameCreate, createDate, userNameUpdate, updateDate, activo
```

### EstadoUsuario

| Codigo | Nombre | Descripcion |
|--------|--------|-------------|
| 01 | Activo | Usuario activo en el sistema |
| 02 | Inactivo | Usuario inactivo temporalmente |
| 03 | Suspendido | Usuario suspendido por incumplimiento |

### Endpoints de Autenticacion

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| POST | `/api/Usuario/login` | Login con email y password | Publico |
| POST | `/api/Usuario/client/loginAndCreate` | Login con Google/Facebook OAuth | Publico |
| POST | `/api/Usuario/register/cliente` | Registro de nuevo cliente | Publico |
| GET | `/api/Usuario/forgot-password/{email}/{host}` | Solicitar reset de password | Publico |
| POST | `/api/Usuario/reset-password` | Cambiar password con token | Publico |
| GET | `/api/Usuario/renew-session` | Renovar token JWT | Autenticado |
| PUT | `/api/Usuario` | Actualizar datos del perfil | Autenticado |
| PUT | `/api/Usuario/telefono` | Actualizar solo el telefono | Autenticado |

### Flujo de Login

```
1. POST /api/Usuario/login
   Body: { applicationCode: "Proveedor"|"Operador", userName, password, rememberMe }

2. Backend valida credenciales con ASP.NET Identity

3. Si valido, genera JWT con claims:
   UserId, UserIdNegocio, UserName, DisplayName, Telefono, Roles

4. Frontend guarda token en localStorage

5. TokenInterceptor inyecta "Authorization: Bearer {token}" en cada request

6. Si token expira en <= 5 min → keepAlive() → renew-session
```

### Login con OAuth (Google)

```
1. Frontend ejecuta popup de Google (SDK de Google)
2. Google retorna idToken
3. POST /api/Usuario/client/loginAndCreate
   Body: { applicationCode, idToken, typeValidation: "Google" }
4. Backend valida con Google.Apis.Auth
5. Si es primera vez, crea usuario con rol Cliente
6. Retorna JWT identico al login normal
```

---

## 5. MODULO: PROVEEDOR

### Entidad: Proveedor

```sql
idProveedor       INT IDENTITY PK
idUsuario         UNIQUEIDENTIFIER FK -> AspNetUsers
razonSocial       VARCHAR(255)        -- Nombre de la empresa
ruc               VARCHAR(20)
idTipoProveedor   INT FK              -- Empresa(01) | Persona Natural(02)
idEstadoProveedor INT FK              -- Estado del registro
telefono          VARCHAR(20)
facebook          VARCHAR(200)
instagram         VARCHAR(200)
-- Audit: userNameCreate, createDate, ...
```

### EstadoProveedor

| Codigo | Nombre | Descripcion |
|--------|--------|-------------|
| 01 | Pendiente | Registro pendiente de aprobacion admin |
| 02 | Aprobado | Proveedor activo y operativo |
| 03 | Rechazado | Solicitud rechazada |
| 04 | Suspendido | Proveedor suspendido temporalmente |

### Entidad: ConfiguracionProveedor

Politicas de negocio de cada proveedor. **Una configuracion por proveedor (UNIQUE)**.

```sql
idConfiguracionProveedor    INT IDENTITY PK
idProveedor                 INT FK UNIQUE

-- Pre-reserva
duracionPreReserva          INT              -- Horas que tiene el cliente para confirmar (default: 12)

-- Politicas de adelanto
porcentajeAdelantoMinimo    DECIMAL(5,2)     -- % minimo del total que debe adelantar (default: 50%)

-- Politicas de cancelacion
tiempoLimiteCancelacion     INT              -- Horas antes para cancelar sin penalidad (default: 24)
porcentajeDevolucionCompleto DECIMAL(5,2)   -- % devuelto si cancela antes del limite (default: 100%)
porcentajeDevolucionParcial  DECIMAL(5,2)   -- % devuelto si cancela despues del limite (default: 50%)
-- Audit: userNameCreate, createDate, ...
```

### Reglas de Negocio: ConfiguracionProveedor

1. **Pre-reserva**: Al crear una reserva, el sistema calcula `FechaExpiracion = ahora + duracionPreReserva horas`. Si el operador no confirma antes, la reserva expira automaticamente.

2. **Adelanto minimo**: Al confirmar una reserva, si el cliente paga un adelanto, debe ser como minimo el `porcentajeAdelantoMinimo` del total. Ejemplo: total S/100, minimo 50% → debe pagar al menos S/50.

3. **Politica de cancelacion**:
   - Si cancela con mas de `tiempoLimiteCancelacion` horas de anticipacion → reembolso de `porcentajeDevolucionCompleto`%
   - Si cancela con menos horas → reembolso de `porcentajeDevolucionParcial`%

### Endpoints

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| GET | `/api/Proveedor/{id}` | Obtener proveedor | Auth |
| PUT | `/api/Proveedor` | Actualizar datos del proveedor | Proveedor |
| GET | `/api/ConfiguracionProveedor/{idProveedor}` | Obtener configuracion | Auth |
| PUT | `/api/ConfiguracionProveedor` | Actualizar configuracion | Proveedor |

---

## 6. MODULO: CANCHA

### Entidad: Cancha

```sql
idCancha          INT IDENTITY PK
idProveedor       INT FK -> Proveedor
idTipoSuperficie  INT FK -> TipoSuperficie
idEstadoCancha    INT FK -> EstadoCancha
codigo            VARCHAR(20) UNIQUE  -- Generado: "PROV001-C01"
nombre            VARCHAR(200)
descripcion       TEXT
precio            DECIMAL(10,2)       -- Precio base referencial
telefonoCancha    VARCHAR(15)

-- Ubicacion
direccion         VARCHAR(500)
codigoUbigeo      CHAR(6) FK -> Ubigeo
latitud           DECIMAL(10,8)
longitud          DECIMAL(10,8)

-- Caracteristicas fisicas
capacidadJugadores INT
tieneTecho         BIT
tieneIluminacion   BIT

-- Audit: userNameCreate, createDate, ...
```

### EstadoCancha

| Codigo | Nombre | Descripcion |
|--------|--------|-------------|
| 01 | Aprobado | Cancha activa, aparece en busquedas |
| 02 | Pendiente | Registro pendiente de aprobacion del admin |
| 03 | Rechazado | Cancha rechazada, no se puede activar |
| 04 | Suspendido | Temporalmente fuera de servicio |
| 05 | Mantenimiento | En reparacion o mejoras |

**Solo las canchas en estado Aprobado (01) son visibles en las busquedas publicas.**

### Flujo de Vida de una Cancha

```
[Proveedor registra cancha]
         │
         ▼
    Pendiente (02)
         │
    [Admin revisa]
    /            \
Rechazado(03)   Aprobado(01) ──── Disponible para reservas
                    │
           ┌────────┴────────┐
      Suspendido(04)    Mantenimiento(05)
```

### Codigo de Cancha (Stored Procedure)

Generado por `sp_GenerarCodigoCancha(@idProveedor)`:

```
Formato: PROVXXX-CYY
Ejemplo: PROV001-C01 (primer cancha del proveedor 1)
         PROV001-C02 (segunda cancha del proveedor 1)
         PROV023-C01 (primer cancha del proveedor 23)
```

### Sistema de Horarios: Expansion a Bloques de 30 Minutos

**Concepto clave**: El frontend trabaja en horas completas, el backend internamente trabaja en bloques de 30 minutos.

**Por que bloques de 30 min?** Permite reservas de 30, 60, 90, 120 minutos de forma flexible.

**Expansion al guardar** (`HorarioCanchaService.ExpandirHorariosCreate`):

```
Frontend envia:   Lunes 09:00 → precio S/50
Backend guarda:   Lunes 09:00-09:30 → precio S/25 (bloque 1)
                  Lunes 09:30-10:00 → precio S/25 (bloque 2)
```

**Compresion al mostrar** (`HorarioCanchaService.ComprimirHorarios`):

```
Backend tiene:    Lunes 09:00-09:30 (S/25) + 09:30-10:00 (S/25)
Frontend recibe:  Lunes 09:00-10:00 (S/50)
```

### Entidades Relacionadas a Cancha

```
Cancha
  ├── TipoDeporteCancha[]  ← N:M Deportes que se pueden jugar (activos)
  │     └── TipoDeporte
  ├── ServicioCancha[]     ← N:M Servicios incluidos (esIncluido = true por defecto)
  │     └── Servicio
  ├── HorarioCancha[]      ← Horarios por dia de semana (bloques de 30 min)
  ├── ImagenCancha[]       ← Fotos (min 1, max 4, una es esPrincipal)
  ├── BloqueoHorario[]     ← Franjas bloqueadas por el proveedor
  ├── OperadorCancha[]     ← Operadores asignados a esta cancha
  ├── CanchaFavorita[]     ← Usuarios que la marcaron favorita
  └── Reserva[]            ← Todas las reservas
```

### Entidad: HorarioCancha

```sql
idHorarioCancha   INT IDENTITY PK
idCancha          INT FK
idDiaSemana       INT FK -> DiaSemana  (1=Lunes ... 7=Domingo)
idHoraInicio      INT FK -> Hora       (ej: idHora=19 → 09:00)
idHoraFin         INT FK -> Hora       (ej: idHora=20 → 09:30)
precioHora        DECIMAL(10,2)        -- Precio del bloque de 30 min
-- CONSTRAINT: UQ (idCancha, idDiaSemana, idHoraInicio) → no duplicados
-- CONSTRAINT: precioHora > 0
-- Audit: userNameCreate, createDate, ...
```

### Entidad: BloqueoHorario

```sql
idBloqueoHorario  INT IDENTITY PK
idCancha          INT FK
fechaBloqueo      DATE                 -- Fecha especifica del bloqueo
idHoraInicio      INT FK -> Hora
idHoraFin         INT FK -> Hora
motivo            VARCHAR(500)         -- Mantenimiento, Clima, Evento especial, etc.
-- Audit: userNameCreate, createDate, ...
```

### Entidad: ImagenCancha

```sql
idImagenCancha    INT IDENTITY PK
idCancha          INT FK
urlImagen         VARCHAR(500)         -- URL en storage (AWS S3)
esPrincipal       BIT                  -- La imagen destacada de la cancha
-- Audit: userNameCreate, createDate, ...
```

### Endpoints de Cancha

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| POST | `/api/Cancha` | Crear cancha | Proveedor |
| PUT | `/api/Cancha` | Actualizar cancha | Proveedor |
| DELETE | `/api/Cancha/{id}` | Eliminar cancha (soft delete) | Proveedor |
| GET | `/api/Cancha/{id}` | Obtener cancha con detalle completo | Auth |
| POST | `/api/Cancha/search` | Busqueda paginada con filtros | Publico/Auth |
| GET | `/api/Cancha/list/{idProveedor}` | Listar canchas de un proveedor | Auth |
| GET | `/api/Cancha/selectcombo` | Para combos/dropdowns | Auth |
| POST | `/api/Cancha/{id}/imagenes` | Subir imagenes (multipart) | Proveedor |
| DELETE | `/api/Cancha/imagenes/{idImagen}` | Eliminar una imagen | Proveedor |

### Logica: Crear Cancha (`CreateCanchaCommandHandler`)

```
1. Buscar estado inicial: Pendiente (02)
2. Expandir horarios recibidos → bloques de 30 min
3. Mapear DTO → Entidad Cancha
4. Asignar estado: Pendiente
5. Agregar TipoDeporteCancha[] (N:M)
6. Agregar ServicioCancha[] con esIncluido = true
7. Generar codigo → sp_GenerarCodigoCancha(@idProveedor)
8. Persistir (UnitOfWork hace commit + audit trail)
9. Retornar GetCanchaDto
```

### Logica: Actualizar Cancha (`UpdateCanchaCommandHandler`)

```
1. Cargar cancha actual con colecciones
2. Si vienen horarios:
   a. Expandir nuevos horarios → ExpandirHorariosUpdate()
   b. Merge inteligente: update existentes, insert nuevos
3. TipoDeportes: desactivar los que ya no vienen, activar/insertar nuevos
4. Servicios: igual que TipoDeportes
5. Mapear DTO → Entidad (sobreescribir campos basicos)
6. Persistir
```

### Logica: Busqueda de Canchas (`SearchCanchaQueryHandler`)

Filtros disponibles:

| Filtro | Descripcion |
|--------|-------------|
| `Nombre` | Contains (case-insensitive) |
| `CodigoUbigeo` | StartsWith (ej: "1501" filtra todos los distritos de Lima) |
| `IdTipoDeporte` | Canchas que admiten ese deporte |
| `IdEstadoCancha` | Por estado (por defecto solo Aprobadas) |
| `Latitud/Longitud + Radio` | Bounding box geografico |
| `Fecha` | Solo canchas con horarios configurados para ese dia |
| `SoloFavoritos + IdUsuario` | Solo favoritas del usuario |
| `IdProveedor` | Solo canchas de ese proveedor |

**Si se filtra por fecha futura**: incluye solo canchas con `HorarioCancha` activos para ese dia de la semana.

**Si la fecha es hoy**: ademas filtra horarios cuya hora de inicio aun no ha pasado.

**Para cada cancha en el resultado**: lanza `GetCanchaByFechaQuery` en paralelo para obtener horarios disponibles del dia.

---

## 7. MODULO: OPERADOR

### Entidad: Operador

```sql
idOperador    INT IDENTITY PK
idUsuario     UNIQUEIDENTIFIER FK -> AspNetUsers  -- Cuenta de login del operador
idProveedor   INT FK -> Proveedor                 -- A que proveedor pertenece
-- Audit: userNameCreate, createDate, ...
```

### Entidad: OperadorCancha (asignaciones)

```sql
idOperadorCancha  INT IDENTITY PK
idOperador        INT FK -> Operador
idCancha          INT FK -> Cancha
activo            BIT
-- CONSTRAINT: UNIQUE (idOperador, idCancha)
```

### Reglas de Negocio

1. Un operador pertenece a **un solo proveedor**.
2. Un operador puede estar asignado a **multiples canchas** del mismo proveedor.
3. En el calendario y dashboard, el operador **solo ve las canchas que tiene asignadas**.
4. El operador **no puede gestionar otros operadores** ni la configuracion del proveedor.
5. Un operador es tambien un usuario de `AspNetUsers` con rol `Operador`.

### Endpoints

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| POST | `/api/Operador` | Crear operador + usuario | Proveedor |
| PUT | `/api/Operador` | Actualizar operador | Proveedor |
| DELETE | `/api/Operador/{id}` | Eliminar operador (soft delete) | Proveedor |
| GET | `/api/Operador/{id}` | Obtener operador | Proveedor |
| POST | `/api/Operador/search` | Buscar operadores del proveedor | Proveedor |

---

## 8. MODULO: CALENDARIO

### Proposito

Vista centralizada de disponibilidad y reservas para el panel admin. Permite:
- Ver slots disponibles/reservados/bloqueados por semana
- Crear reservas directamente desde el calendario
- Buscar clientes existentes o crear nuevos al vuelo
- Validar disponibilidad en tiempo real

### Endpoints

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| GET | `/api/Calendario/canchas-usuario` | Canchas segun rol del usuario | Auth |
| POST | `/api/Calendario/disponibilidad-semanal` | Grid de disponibilidad semanal | Auth |
| POST | `/api/Calendario/crear-reserva-operador` | Crear reserva desde el panel | Operador/Proveedor |
| GET | `/api/Calendario/buscar-cliente` | Autocomplete de clientes | Auth |
| GET | `/api/Calendario/horas-disponibles` | Horas libres de una cancha en una fecha | Auth |
| POST | `/api/Calendario/validar-disponibilidad` | Validar antes de confirmar | Auth |

### Logica: Canchas del Usuario (`GetCanchasUsuarioQueryHandler`)

```
Si rol == Proveedor:
  → Todas las canchas activas donde idProveedor == UserIdNegocio

Si rol == Operador:
  → Busca en OperadorCancha los idCancha asignados al operador
  → Carga esas canchas

Para cada cancha: resuelve primer TipoDeporteCancha activo
Retorna ordenado por nombre
```

### Logica: Horas Disponibles (`GetCanchaByFechaQueryHandler`)

```
1. Convertir fecha → dia de semana en espanol (ej: lunes)
2. Buscar HorarioCancha activos de esa cancha para ese dia
3. Buscar Reservas activas de esa cancha para esa fecha con sus DetalleReserva
4. Construir HashSet con IdHorarioCancha ya reservados
5. Filtrar: descartar los que esten en el HashSet
6. Si la fecha es HOY: descartar ademas los horarios cuya hora de inicio ya paso
7. Retornar lista ordenada por hora de inicio con precio por bloque
```

### Logica: Crear Reserva Operador (`CrearReservaOperadorCommandHandler`)

Permite al operador crear una reserva desde el panel. Soporta clientes nuevos o existentes.

```
1. ObtenerOCrearCliente:
   a. Si viene IdCliente → buscar en AspNetUsers
   b. Si EsNuevoCliente = true → ejecutar CreateUsuarioCommand (crea usuario + rol Cliente)

2. ValidarDisponibilidadHorarios:
   a. Para cada bloque en el request, buscar HorarioCancha activos en el rango [IdHorarioCanchaInicio, IdHorarioCanchaFin]
   b. Verificar que ningun DetalleReserva existente con estado Confirmado o Pendiente en la misma fecha use esos horarios
   c. Si hay conflicto → error

3. Generar codigo de reserva (SP o fallback: "RES-{año}-{numero}")

4. Determinar tipo de reserva:
   Inmediata → estado: Confirmado, registra operador + fechaConfirmacion
   PreReserva → estado: Pendiente

5. Crear registro Pago:
   montoAdelanto / montoPendiente segun adelanto recibido
   estado: Pendiente / Parcial / Pagado

6. Persistir Reserva + Pago

7. Crear DetalleReserva: un registro por cada HorarioCancha del rango seleccionado

8. Retornar ReservaOperadorResponseDto con todos los datos
```

### Estructura de Disponibilidad Semanal

**Request**:
```json
{
  "idCancha": 5,
  "fechaInicio": "2025-11-25",
  "fechaFin": "2025-12-01"
}
```

**Response por dia**:
```json
{
  "fecha": "2025-11-25",
  "dia": "Lunes",
  "slots": [
    {
      "idHorarioCancha": 42,
      "horaInicio": "09:00",
      "horaFin": "09:30",
      "precio": 25.00,
      "estado": "DISPONIBLE | RESERVADO | BLOQUEADO",
      "reserva": {
        "idReserva": 15,
        "cliente": "Juan Perez",
        "estadoReserva": "Pendiente",
        "estadoPago": "Pendiente"
      }
    }
  ]
}
```

---

## 9. MODULO: RESERVA

### Entidad: Reserva

```sql
idReserva                 INT IDENTITY PK
codigoReserva             VARCHAR(50) UNIQUE        -- Generado: "RES-2025-000001"
idCliente                 UNIQUEIDENTIFIER FK -> AspNetUsers
idCancha                  INT FK -> Cancha
idTipoDeporte             INT FK -> TipoDeporte     -- Deporte para el que reserva
fechaReserva              DATETIMEOFFSET            -- Fecha y hora del inicio
montoTotal                DECIMAL(10,2)
fechaExpiracionPreReserva DATETIMEOFFSET            -- Deadline para confirmar (null si ya confirmo)
notificacionAdvertenciaEnviada BIT                 -- Para evitar duplicar alertas de expiracion
recordatorioEnviado       BIT                       -- Para evitar duplicar recordatorios 1h antes
idEstadoReserva           INT FK -> EstadoReserva

-- Confirmacion
idOperadorConfirmo        INT FK -> Operador        -- Quien confirmo (null si aun no)
fechaConfirmacion         DATETIMEOFFSET

observaciones             TEXT
-- Audit: userNameCreate, createDate, ...
```

### Entidad: DetalleReserva

```sql
idDetalleReserva  INT IDENTITY PK
idReserva         INT FK -> Reserva
idHorarioCancha   INT FK -> HorarioCancha  -- Bloque de 30 min reservado
activo            BIT
```

Una reserva puede ocupar **multiples bloques** (ej: 2 horas = 4 registros de DetalleReserva).

### Codigo de Reserva (Stored Procedure)

Generado por `sp_GenerarCodigoReserva`:

```
Formato: RES-YYYY-NNNNNN
Ejemplo: RES-2025-000001 (primera reserva del año 2025)
         RES-2025-000042 (reserva numero 42)
```

### EstadoReserva

| Codigo | Nombre | Descripcion |
|--------|--------|-------------|
| 01 | Pendiente | Creada, esperando confirmacion del operador |
| 02 | Confirmado | Pago confirmado, reserva activa |
| 03 | Completado | Se realizo exitosamente (pasado) |
| 04 | Cancelado | Cancelada por cliente/operador/sistema |
| 05 | No Presentado | Cliente no asistio |

### Ciclo de Vida de una Reserva

```
[Cliente crea desde la app]
         │
         ▼
    Pendiente (01)
    + FechaExpiracion calculada
    + Notificacion a operadores
         │
    ┌────┴─────────────────────┐
    │                          │
[Operador confirma]    [Expira FechaExpiracion]
    │                          │
    ▼                          ▼
Confirmado (02)          Expirado/Cancelado (04)
    │                    [BackgroundService lo procesa]
    │
    │─── [1h antes] ──→ Recordatorio al cliente
    │
    ├── [Cliente no llega] ──→ No Presentado (05)
    │
    └── [Se realiza] ──→ Completado (03)

[En cualquier estado activo]
    └── [Cancelacion manual] ──→ Cancelado (04)
                                 + Calculo de reembolso
```

### Endpoints

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| POST | `/api/Reserva` | Crear reserva (app cliente) | Cliente |
| PUT | `/api/Reserva` | Actualizar reserva | Auth |
| DELETE | `/api/Reserva/{id}` | Eliminar reserva | Auth |
| GET | `/api/Reserva/{id}` | Obtener detalle | Auth |
| POST | `/api/Reserva/list` | Listar reservas | Auth |
| POST | `/api/Reserva/search` | Busqueda paginada con filtros | Auth |
| POST | `/api/Reserva/confirmar-reserva-operador` | Confirmar + registrar pago | Operador |
| POST | `/api/Reserva/liberar-reserva-operador` | Cancelar/liberar reserva | Operador |
| GET | `/api/Reserva/pendientes-operador/{idProveedor}` | Pendientes con urgencia | Operador |
| POST | `/api/Reserva/mis-reservas/{idUsuario}` | Historial del cliente | Cliente |

### Logica: Crear Reserva desde App Cliente (`CreateReservaCommandHandler`)

```
1. Cargar cancha con proveedor, configuracion, operadores
2. Buscar metodo de pago por codigo
3. Cargar todas las reservas activas de esa cancha en esa fecha (excluye Cancelado/Expirado)
4. VALIDAR CONFLICTOS:
   - HashSet de IdHorarioCancha ya reservados
   - Si algun IdHorarioCancha del request esta en el HashSet → ERROR con IDs conflictivos
5. Crear entidad Reserva via AutoMapper
6. Agregar DetalleReserva (un registro por cada IdHorarioCancha)
7. AJUSTAR FechaReserva: combinar fecha enviada con hora de inicio del primer horario
8. CALCULAR EXPIRACION:
   FechaExpiracion = ahora + ConfiguracionProveedor.DuracionPreReserva horas (default: 12h)
9. Estado inicial: Pendiente
   RecordatorioEnviado = false
   NotificacionAdvertenciaEnviada = false
10. Generar codigo (SP o fallback)
11. Persistir Reserva + DetalleReserva
12. Crear Pago:
    monto = montoTotal
    montoAdelanto = 0
    montoPendiente = montoTotal
    estado = Pendiente
13. Persistir Pago
14. NOTIFICAR OPERADORES (email + WhatsApp):
    - Si hay operadores asignados → notificar a todos
    - Si no hay operadores → notificar al proveedor (fallback)
    - Errores de notificacion NO interrumpen la reserva
15. Retornar ReservaConPagoDto con info del operador, telefono cancha y mensaje de expiracion
```

### Logica: Confirmar Reserva (`ConfirmarReservaOperadorCommandHandler`)

```
1. Cargar reserva con estado, cancha y pagos
2. VALIDAR: reserva existe y esta activa
3. VALIDAR: estado == Pendiente (no confirmar dos veces)
4. VALIDAR: FechaExpiracion > ahora (no expirada)
5. Buscar pago activo
6. Obtener porcentajeAdelantoMinimo del proveedor
7. VALIDAR ADELANTO:
   a. MontoAdelanto <= MontoTotal
   b. Si MontoAdelanto > 0: (MontoAdelanto / MontoTotal * 100) >= porcentajeAdelantoMinimo
8. Determinar estado del pago:
   adelanto >= total   → PAGADO
   adelanto > 0        → PARCIAL
   adelanto == 0       → PENDIENTE
9. Actualizar pago (montoAdelanto, montoPendiente, numeroReferencia, estadoPago)
10. Cambiar reserva a CONFIRMADO
11. Anular FechaExpiracionPreReserva (ya no expira)
12. NOTIFICAR CLIENTE: email + WhatsApp con detalle, adelanto y saldo pendiente
```

### Logica: Cancelar Reserva (`LiberarReservaOperadorCommandHandler`)

```
1. Cargar reserva con cadena completa (cancha → proveedor → configuracion)
2. VALIDAR: estado == Pendiente O Confirmado
3. Cargar pago activo
4. SI estado == Confirmado (ya pago algo):
   a. Calcular horas hasta la fecha de reserva
   b. Si horas > TiempoLimiteCancelacion:
      porcentajeReembolso = PorcentajeDevolucionCompleto (default: 100%)
   c. Si horas <= TiempoLimiteCancelacion:
      porcentajeReembolso = PorcentajeDevolucionParcial (default: 50%)
   d. VALIDAR: MontoReembolso enviado == MontoAdelanto * porcentaje (tolerancia S/0.01)
   e. Actualizar pago: estado = CANCELADO, montoReembolso registrado
5. SI estado == Pendiente:
   VALIDAR: MontoReembolso == 0 (no pago nada aun)
6. Cambiar reserva a CANCELADO
7. Anular FechaExpiracionPreReserva
8. Retornar mensaje indicando si hubo reembolso
```

### Logica: Reservas Pendientes con Urgencia (`ReservasPendientesOperadorQueryHandler`)

Devuelve las reservas pendientes con su nivel de urgencia para el operador:

```
Filtro: cancha.IdProveedor == idProveedor, estado = Pendiente, activas

Para cada reserva:
  HorasRestantes = (FechaExpiracion - ahora).TotalHours

  NivelUrgencia:
    < 0 horas    → "Expirada"
    <= 6 horas   → "CRITICA"
    <= 24 horas  → "ALTA"
    > 24 horas   → "MEDIA"

Ordenar: por FechaExpiracion ASC (las mas urgentes primero)
```

### Filtros de Busqueda de Reservas (SearchReserva)

```json
{
  "idProveedor": 5,
  "idCancha": 12,
  "estadoReserva": "01",
  "estadoPago": "03",
  "fechaDesde": "2025-11-01",
  "fechaHasta": "2025-11-30",
  "nombreCliente": "Juan"
}
```

---

## 10. MODULO: PAGO

### Entidad: Pago

```sql
idPago              INT IDENTITY PK
idReserva           INT FK -> Reserva
moneda              CHAR(3)            -- "PEN" (soles)
codigoOperacion     VARCHAR(30)        -- Codigo Yape/Plin/transferencia
numeroReferencia    VARCHAR(50)        -- Numero de boleta/recibo efectivo
monto               DECIMAL(10,2)      -- Monto total de la reserva
montoAdelanto       DECIMAL(10,2)      -- Cuanto pago el cliente
montoPendiente      DECIMAL(10,2)      -- Cuanto falta
montoReembolso      DECIMAL(10,2)      -- En caso de cancelacion
idMetodoPago        INT FK -> MetodoPago
idEstadoPago        INT FK -> EstadoPago

-- Integracion Culqi (para planes, NO reservas)
culqiChargeId       NVARCHAR(100)
culqiTokenId        NVARCHAR(100)
culqiReferenceCode  NVARCHAR(50)

idOperador          INT FK -> Operador  -- Quien registro el pago
-- Audit: userNameCreate, createDate, ...
```

### MetodoPago

| Codigo | Nombre | Uso |
|--------|--------|-----|
| 01 | Tarjeta | No implementado para reservas |
| 02 | Efectivo | **Unico metodo para reservas** |
| 03 | Transferencia | Usado con Culqi (planes) |
| 04 | Yape | Usado con Culqi (planes) |
| 05 | Plin | Usado con Culqi (planes) |

### EstadoPago

| Codigo | Nombre | Descripcion |
|--------|--------|-------------|
| 01 | Pagado | Pago completo |
| 02 | Parcial | Adelanto parcial (efectivo) |
| 03 | Pendiente | Sin pagar |
| 04 | Cancelado | Pago devuelto/reembolsado |
| 05 | Rechazado | Pago rechazado (gateway) |

### Comprobante de Pago

```sql
idComprobantePago   INT IDENTITY PK
idPago              INT FK -> Pago
numeroComprobante   VARCHAR(50) UNIQUE
tipoComprobante     VARCHAR(20)    -- Recibo | Boleta | Factura
urlPDF              VARCHAR(500)
```

### Endpoints

| Metodo | Endpoint | Descripcion | Rol |
|--------|----------|-------------|-----|
| POST | `/api/Pago/confirmar` | Confirmar pago con codigo operacion | Operador |
| POST | `/api/Pago/completar-pago` | Completar saldo pendiente (efectivo) | Operador |

### Logica: Confirmar Pago (`ConfirmarPagoCommandHandler`)

Para pagos con codigo de operacion (Yape/Plin/transferencia):

```
1. Cargar pago con estado, metodo y reserva
2. VALIDAR: estado == Pendiente
3. VALIDAR EXPIRACION: CreateDate + minutosExpiracion(default 15) > ahora
4. VALIDAR CODIGO OPERACION segun metodo:
   Yape           → QrCodeService.ValidarCodigoOperacionYape()
   Plin           → QrCodeService.ValidarCodigoOperacionPlin()
   Otros          → minimo 6 caracteres
5. Cambiar estado pago → PAGADO
6. Guardar codigoOperacion
7. Si tiene reserva → cambiar reserva → CONFIRMADO
```

### Logica: Completar Pago (`CompletarPagoCommandHandler`)

Para cuando el cliente paga el saldo pendiente en efectivo:

```
1. Cargar pago con navegaciones
2. VALIDAR: metodo == Efectivo (solo efectivo tiene parciales)
3. VALIDAR: estado == Parcial
4. VALIDAR: MontoRestante enviado == MontoPendiente exactamente
5. Actualizar:
   montoAdelanto += montoRestante
   montoPendiente = 0
   numeroReferencia = nuevoNumero (si se proporciona)
   estado = PAGADO
6. Si reserva asociada no esta CONFIRMADA → confirmarla
```

### Integracion Culqi (para Planes de Proveedores)

> **IMPORTANTE**: Culqi solo se usa para el pago de planes del proveedor (suscripciones), NO para reservas de canchas.

**CulqiWebhookController** (`POST /api/culqi/webhook`):

| Evento Culqi | Accion en el sistema |
|-------------|---------------------|
| `charge.succeeded` | Busca pago por `culqiChargeId` → estado **Pagado** |
| `charge.failed` | Busca pago por `culqiChargeId` → estado **Rechazado** |
| `order.status.changed` state=`paid` | Para Yape/Plin QR → estado **Pagado** |
| `order.status.changed` state=`expired/deleted` | Para Yape/Plin QR → estado **Rechazado** |

**Limites Culqi/Yape**:
- Monto maximo: S/ 2,000 por transaccion
- Codigo de aprobacion: valido 2 minutos
- Moneda: solo PEN (soles)

---

## 11. MODULO: NOTIFICACIONES

### Entidad: Notificacion

```sql
idNotificacion  INT IDENTITY PK
idUsuario       UNIQUEIDENTIFIER FK -> AspNetUsers
titulo          VARCHAR(200)
mensaje         TEXT
tipo            VARCHAR(50)   -- INFO | ALERTA | RECORDATORIO | CONFIRMACION
leido           BIT           -- 0 = No leido, 1 = Leido
idReserva       INT FK -> Reserva (opcional)
fechaCreacion   DATETIMEOFFSET
fechaLeido      DATETIMEOFFSET
activo          BIT
```

### NotificacionService: Tipos de Notificacion

| Metodo | Destinatario | Canal | Cuando |
|--------|-------------|-------|--------|
| `NotificarNuevaReservaPendiente` | Operadores asignados | Email + WhatsApp | Al crear reserva |
| `NotificarReservaConfirmada` | Cliente | Email + WhatsApp | Al confirmar reserva |
| `NotificarReservaProximaExpirar` | Operadores asignados | Email + WhatsApp | 6h antes de expirar |
| `NotificarReservaExpirada` | Operadores asignados | Email + WhatsApp | Al expirar |
| `NotificarRecordatorioReserva` | Cliente | Email + WhatsApp | 1h antes de la reserva |

### Fallback de Destinatario

Si una cancha **no tiene operadores asignados**, las notificaciones se envian al **Proveedor** como fallback.

### Formato de Horarios en Notificaciones

`FormatearHorariosConsecutivos` agrupa bloques de 30 min consecutivos:

```
Bloques: 08:00-08:30, 08:30-09:00, 10:00-10:30
Resultado: "08:00 - 09:00, 10:00 - 10:30 (3 horas)"
```

---

## 12. JOBS EN BACKGROUND

### ReservaExpirationService

Servicio que se ejecuta **cada 30 minutos** con 3 tareas:

#### Tarea 1: ProcesarReservasExpiradas

```
Buscar: estado == Pendiente AND FechaExpiracion <= ahora
Para cada una:
  → Cambiar estado → Expirado (04)
  → Notificar operadores (email + WhatsApp)
```

#### Tarea 2: NotificarReservasProximasExpirar

```
Buscar: estado == Pendiente AND FechaExpiracion <= ahora+6h AND NotificacionAdvertenciaEnviada == false
Para cada una:
  → Notificar operadores: "Esta reserva expira en menos de 6 horas"
  → Marcar NotificacionAdvertenciaEnviada = true (evita duplicar)
```

#### Tarea 3: EnviarRecordatoriosReservasProximas

```
Buscar: estado == Confirmado AND fecha == hoy AND RecordatorioEnviado == false
Para cada una:
  → Obtener primer horario de la reserva
  → Calcular tiempo hasta la reserva
  → Si falta entre 0 y 60 minutos:
      → Enviar recordatorio al cliente (email + WhatsApp)
      → Marcar RecordatorioEnviado = true (evita duplicar)
```

---

## 13. FLUJOS COMPLETOS DE NEGOCIO

### Flujo 1: Reserva completa desde app cliente

```
CLIENTE (App Web)
│
├─1. Busca canchas disponibles
│     POST /api/Cancha/search
│     Filtros: ubicacion, deporte, fecha, disponibilidad
│
├─2. Selecciona cancha y ve horarios disponibles
│     GET /api/Calendario/horas-disponibles?idCancha=X&fecha=Y
│     → Solo slots no reservados
│     → Si es HOY: solo horarios que no pasaron
│
├─3. Selecciona horarios y crea reserva
│     POST /api/Reserva
│     Body: { idCancha, idTipoDeporte, fecha, idHorarioCanchas[], idMetodoPago }
│
│     BACKEND:
│     ├── Validar conflictos de horarios (concurrencia)
│     ├── Crear Reserva (PENDIENTE) + DetalleReserva[]
│     ├── Calcular FechaExpiracion (config proveedor, default 12h)
│     ├── Crear Pago (PENDIENTE, montoAdelanto=0)
│     └── Notificar operadores → email + WhatsApp
│
├─4. Cliente recibe:
│     - Codigo de reserva (RES-2025-000042)
│     - Fecha de expiracion
│     - Telefono del operador/cancha para coordinar
│
OPERADOR (Panel Admin)
│
├─5. Ve notificacion de nueva reserva pendiente
│     GET /api/Reserva/pendientes-operador/{idProveedor}
│     → Lista con nivel de urgencia: CRITICA/ALTA/MEDIA
│
│     [Background cada 30min]
│     → Si expira en < 6h: nueva notificacion "CRITICA"
│     → Si FechaExpiracion paso: → EXPIRADO automaticamente
│
├─6. Contacta al cliente (telefono del cliente en la reserva)
│
├─7. Confirma la reserva con adelanto
│     POST /api/Reserva/confirmar-reserva-operador
│     Body: { idReserva, montoAdelanto, numeroReferencia }
│
│     BACKEND:
│     ├── Validar estado == Pendiente
│     ├── Validar no expirada
│     ├── Validar adelanto >= minimo configurado
│     ├── Actualizar Pago (PARCIAL / PAGADO segun adelanto)
│     ├── Cambiar Reserva → CONFIRMADO
│     └── Notificar cliente → email + WhatsApp
│
DIA DE LA RESERVA
│
├─8. [Background] 1h antes: recordatorio al cliente
│     → email + WhatsApp con horarios formateados
│
├─9. Cliente llega y juega
│
├─10. Si habia saldo pendiente, el cliente paga el resto
│      POST /api/Pago/completar-pago
│      Body: { idPago, montoRestante, numeroReferencia }
│      → Pago → PAGADO, Reserva → CONFIRMADO
│
└─11. Reserva pasa a COMPLETADO (manual o automatico)
```

### Flujo 2: Reserva manual desde el panel (operador)

```
OPERADOR
│
├─1. Va al calendario /admin/calendario
│
├─2. Selecciona cancha y semana
│     GET /api/Calendario/canchas-usuario
│     POST /api/Calendario/disponibilidad-semanal
│
├─3. Hace click en slot DISPONIBLE
│
├─4. Modal: busca cliente
│     GET /api/Calendario/buscar-cliente?termino=Juan
│     → Autocomplete por nombre, apellido o telefono
│     → Si no existe: crea nuevo con nombre y telefono
│
├─5. Selecciona duracion (1 o 2 horas), metodo de pago, adelanto
│
├─6. Valida disponibilidad en tiempo real
│     POST /api/Calendario/validar-disponibilidad
│
├─7. Crea la reserva
│     POST /api/Calendario/crear-reserva-operador
│     Tipos:
│       Inmediata → Reserva queda CONFIRMADA directo
│       PreReserva → Reserva queda PENDIENTE
│
└─8. Calendario se actualiza: slot pasa de VERDE a AZUL
```

### Flujo 3: Cancelacion con reembolso

```
OPERADOR
│
├─1. Busca la reserva en /admin/reservas o en el calendario
│
├─2. Click en "Cancelar reserva"
│
├─3. Sistema calcula la politica del proveedor:
│     - FechaReserva - ahora = N horas
│     - Si N > TiempoLimiteCancelacion (24h):
│         reembolso = MontoAdelanto * PorcentajeDevolucionCompleto (100%)
│     - Si N <= TiempoLimiteCancelacion:
│         reembolso = MontoAdelanto * PorcentajeDevolucionParcial (50%)
│
├─4. Operador ingresa motivo y confirma el monto de reembolso
│
├─5. POST /api/Reserva/liberar-reserva-operador
│     Body: { idReserva, motivoCancelacion, montoReembolso }
│
│     BACKEND:
│     ├── Validar que montoReembolso coincide con el calculado (tolerancia S/0.01)
│     ├── Pago → CANCELADO (registra montoReembolso)
│     └── Reserva → CANCELADO
│
└─6. Slot en calendario vuelve a VERDE (disponible)
```

---

## 14. REGLAS DE NEGOCIO GLOBALES

### Sobre Horarios

1. El sistema trabaja internamente con **bloques de 30 minutos** (48 bloques/dia).
2. El frontend ve y envia **bloques de 1 hora** que el backend expande automaticamente.
3. **No se puede reservar un horario en el pasado** (para reservas del dia actual).
4. No se puede reservar si hay **conflicto** con otra reserva activa (ni Pendiente ni Confirmada).

### Sobre Pagos de Reservas

1. Las reservas **solo se pagan en EFECTIVO** cuando el cliente llega.
2. El operador registra el adelanto al confirmar la reserva.
3. El adelanto minimo es configurable por proveedor (default: 50% del total).
4. Un pago puede quedar en estado **Parcial** si el cliente paga una parte y el resto al llegar.

### Sobre Expiracion

1. Al crear una reserva, se calcula automaticamente la `FechaExpiracionPreReserva`.
2. La duracion es configurable en `ConfiguracionProveedor.DuracionPreReserva` (default: 12 horas).
3. El background service corre **cada 30 minutos** para expirar reservas vencidas.
4. Las notificaciones de advertencia se envian cuando falta **menos de 6 horas** para expirar.
5. Los recordatorios al cliente se envian cuando falta entre **0 y 60 minutos** para la reserva.

### Sobre Reembolsos

1. Solo aplican para reservas ya **Confirmadas** (el cliente habia pagado algo).
2. El sistema calcula automaticamente el porcentaje segun la politica del proveedor.
3. El monto calculado debe coincidir exactamente con el enviado (tolerancia de S/ 0.01).

### Sobre Codigos Unicos

| Entidad | Formato | Stored Procedure |
|---------|---------|-----------------|
| Cancha | `PROVXXX-CYY` | `sp_GenerarCodigoCancha(@idProveedor)` |
| Reserva | `RES-YYYY-NNNNNN` | `sp_GenerarCodigoReserva` |

Ambos SP tienen un fallback en el codigo si el SP falla.

### Sobre Notificaciones

1. Los errores de notificacion **NO interrumpen** la operacion principal.
2. Si una cancha no tiene operadores, el proveedor actua como fallback.
3. Los flags `RecordatorioEnviado` y `NotificacionAdvertenciaEnviada` evitan notificaciones duplicadas.

### Sobre Audit Trail

Todos los registros tienen auditoria automatica (gestionada por el UnitOfWork, no se setea manualmente):

```
userNameCreate  → usuario que creo el registro
createDate      → fecha/hora de creacion
userNameUpdate  → ultimo usuario que modifico
updateDate      → fecha/hora de ultima modificacion
activo          → false = soft delete (registro existe pero no se usa)
```

---

## 15. PENDIENTES DE IMPLEMENTACION

### Backend no conectado (datos mockeados en frontend)

| Funcionalidad | Endpoint pendiente | Estado |
|-------------|---------------------|--------|
| Dashboard Proveedor | `GET /api/Dashboard/proveedor` | Mockeado en frontend |
| Dashboard Operador | `GET /api/Dashboard/operador` | Mockeado en frontend |

### Funcionalidades faltantes

| Funcionalidad | Notas |
|-------------|-------|
| Validacion de firma webhook Culqi | Pendiente de documentacion de Culqi |
| Notificacion al cliente al cancelar | TODO en `LiberarReservaOperadorCommandHandler` |
| Usuario en CrearReservaOperador | TODO: hardcodeado (GUID temporal) |
| Pago con Tarjeta | `PagoStrategyFactory` lo detecta como "No implementado" |
| Planes de proveedores |
| Reportes PDF | Mencionado en frontend, no implementado |
| Notificaciones Push | Sistema en tiempo real |
| Sincronizacion WebSocket para calendario | Actualmente usa polling cada 30 segundos |

---

*Documentacion generada para el proyecto ReservaCanchas API*
*Ver tambien: `ARQUITECTURA.md` para detalles de la arquitectura tecnica*
