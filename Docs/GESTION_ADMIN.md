# Gestión Admin: proveedores y revisión de canchas

## Alcance

Módulos de lectura/cambio de estado de proveedores y canchas. Los motivos, la bitácora detallada, las notificaciones y los módulos de suscripciones/pagos/catálogo se mantienen como pendientes en `ReservaCanchas_admin_Web/Docs/ADMIN_PENDIENTES.md`.

## Endpoints

Todos los endpoints `/api/GestionAdmin` requieren JWT Bearer y rol Admin. El dominio vuelve a comprobar ese rol.

| Método | Ruta | Uso |
|---|---|---|
| GET | `/api/GestionAdmin/proveedores` | Listado paginado de todos los estados. |
| GET | `/api/GestionAdmin/proveedores/{id}` | Detalle del proveedor y resumen de su plan/canchas/reservas futuras. |
| PUT | `/api/GestionAdmin/proveedores/{id}/estado` | Activar, suspender o inactivar. |
| GET | `/api/GestionAdmin/canchas` | Listado paginado de canchas activas para revisión. |
| GET | `/api/GestionAdmin/canchas/{id}` | Detalle privado de la cancha. |
| PUT | `/api/GestionAdmin/canchas/{id}/estado` | Cambio de estado de publicación. |
| GET | `/api/GestionNegocio/estado` | Estado del negocio asociado al proveedor/operador autenticado. |
| POST | `/api/GestionNegocio/canchas/{id}/reenviar` | Propietario habilitado reenvía una cancha rechazada. |
| GET | `/api/Cancha/gestion/{id}` | Detalle privado autorizado, incluso si no está publicado. |
| POST | `/api/Cancha/gestion/search` | Listado privado de canchas del proveedor autenticado. |

Filtros GET: `texto` (hasta 150), `estado`, `idProveedor` opcional, `pagina` (desde 1), `tamanoPagina` (1–100; por defecto 20).
Los listados calculan el total en SQL y luego aplican paginación con orden estable.

Cuerpo de cambio/reenvío:

```json
{ "estado": "SUSPENDIDO", "version": "2026-11-17T15:00:00+00:00" }
```

Proveedor: `ACTIVO`, `SUSPENDIDO`, `INACTIVO`. Cancha: códigos del catálogo (`01` aprobada, `02` pendiente, `03` rechazada, `04` publicación suspendida).
La versión corresponde a `UpdateDate ?? CreateDate` del detalle consultado. Se rechaza una versión desactualizada.

## Proveedores y renovaciones

- Habilitado: proveedor activo y `EstadoProveedor.Codigo = 02` (Aprobado).
- Suspendido: proveedor activo con estado `04`.
- Inactivo: `Activo = false`.
- Suspender/inactivar cambia todas las canchas activas a Pendiente, incluso si tenían otro estado de revisión.
- No se borran ni cancelan reservas, pagos, operadores ni la cuenta de usuario.
- Se cancelan renovaciones automáticas y suscripciones externas todavía referenciadas, incluidas referencias anteriores pendientes de cancelación; se evita repetir el mismo identificador externo en un lote.
- Tras confirmar la cancelación: `AutoRenovacion = false`, `CancelAtPeriodEnd = true`, `FechaProximoCobro = null`. La fecha fin y el estado de la contratación se conservan.
- El callback de cancelación de Culqi no convierte en ACTIVE un plan en gracia/suspendido ni reabre una contratación que no era actual.
- Reactivar restablece el proveedor a Aprobado/activo. No vuelve a aprobar canchas ni vuelve a habilitar la renovación.

La pasarela no participa en la transacción SQL. Primero se confirman las cancelaciones externas; si falla alguna, el cambio local se rechaza. Una cancelación externa ya confirmada puede haberse aplicado aunque el lote falle: el mensaje permite reintentar, y la cancelación admite suscripciones inexistentes o verificadas como ya canceladas/finalizadas. La conciliación administrativa completa de incidencias externas figura entre los pendientes.

## Acceso limitado y revisión

`NegocioRestringidoBehavior` consulta el estado actual en el backend para comandos de proveedores/operadores, sin confiar en que el token oculte botones. Permite consultas y atención de reservas existentes: edición de una reserva existente, confirmación/liberación/cancelación y sus cobros/correcciones. Bloquea altas de reservas, cambios de canchas, operadores, planes y demás comandos de negocio. Conserva operaciones de cuenta/perfil y la identidad necesaria para iniciar sesión.

El contexto `ApplicationCode = Cliente` de la misma cuenta no administra al proveedor. La reserva pública se procesa como compra y exige una cancha publicable; la restricción administrativa del negocio no se aplica a compras del usuario en otros negocios.

Una cancha pendiente/rechazada no queda bloqueada para la operación interna de un negocio habilitado. La revisión controla la publicación:

1. Crear cancha → Pendiente.
2. Admin → Aprobada o Rechazada.
3. Rechazada → propietario corrige/guarda → Reenviar → Pendiente.
4. Aprobada → Admin suspende publicación.
5. Publicación suspendida → Admin devuelve a Pendiente → revisión y aprobación.

El formulario general del proveedor no puede cambiar `IdEstadoCancha` ni transferir `IdProveedor`; tampoco puede modificar su propio `IdEstadoProveedor` desde el perfil. Los cambios de imágenes comprueban propiedad y actualizan la versión de la cancha.

## Publicación y concurrencia

`NegocioReglas.Publicable` exige simultáneamente cancha activa/aprobada y proveedor habilitado. La consulta pública no acepta un filtro de propietario para saltarse esa condición. El detalle público y la creación de reservas públicas verifican la misma regla.

Las consultas de gestión se separan de las públicas: el proveedor ve sus canchas pendientes y rechazadas desde la ruta privada. La autorización se comprueba también en el detalle y en la edición de datos/imágenes.

El modelo EF marca estado y última actualización como tokens de concurrencia (y `Proveedor.Activo`), sin agregar columnas. Esto evita sobrescribir cambios administrativos concurrentes; ante conflicto se debe recargar el detalle y revisar nuevamente.

## Despliegue y verificación

- El esquema base ya incluye estado Suspendido `04`. Si falta en una instalación, completar con el script idempotente de `Docs/Sql/ADMIN_ESTADOS_SQLSERVER.sql` o `ADMIN_ESTADOS_MYSQL.sql` según el motor.
- Reiniciar/desplegar API y frontend para que se registren handlers, comportamiento de MediatR y rutas nuevas.
- Los tests cubren cambios de proveedor, fallo/reintento de cancelación, conservación de reservas, versiones, transiciones de cancha, propiedad/reenvío, publicación y acceso limitado.
- Se comprueba la traducción de consultas a SQL Server/MySQL sin conectarse a bases reales. Las pruebas de pasarela usan HTTP simulado, sin cargos ni cancelaciones reales.
