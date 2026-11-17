# Search de Reservas de gestión

El listado existente `/admin/reservas` contiene filtros, indicadores y campos que antes
estaban en Situación de reservas del reporte de movimientos. El código de reserva es
el enlace al detalle; la columna Ver/ojito se eliminó.

## Permisos

- Proveedor: canchas de su negocio, incluidas las inactivas para histórico.
- Operador: canchas asignadas mediante asignaciones activas, de su proveedor y usuario válidos.
- `UserIdNegocio` no se interpreta siempre como IdProveedor: el servidor resuelve el rol y alcance.
- `idProveedor` enviado por clientes anteriores no autoriza ni amplía resultados; se ignora.
- Se comprueba el alcance en el search y en el detalle del ojito.
- Los endpoints de gestión usan explícitamente Bearer y roles Proveedor/Operador.

## Búsqueda

`POST /api/Reserva/search` conserva el contrato de `filter`, `page`, `sort`:

```json
{
  "filter": {
    "fechaDesdeLocal": "2026-11-13",
    "fechaHastaLocal": "2026-11-13",
    "idCancha": null,
    "codigoEstado": null,
    "codigoEstadoPago": null,
    "searchText": null
  },
  "page": { "page": 1, "pageSize": 12 },
  "sort": [{ "property": "createDate", "direction": "desc" }]
}
```

- Fechas locales inclusivas por zona de cancha, con extremo final exclusivo al siguiente día.
- Fechas opcionales en la API; en pantalla Limpiar restablece Desde/Hasta a hoy y borra los demás filtros.
- Los campos anteriores `fechaDesde`/`fechaHasta` siguen aceptándose como fechas de calendario.
- Cancha por ID; el filtro anterior por nombre sigue disponible dentro del alcance autorizado.
- Texto por código, nombre/apellidos o teléfono.
- Estados del catálogo, más `VIGENTES` y `ANULADAS`.
- Reserva sin Pago se considera pendiente de cobro y conserva precio/saldo.
- Las anuladas (`Activo=false`) se muestran como canceladas; forman parte del conteo de canceladas.
- Página de hasta 200 filas; la pantalla ofrece 12/24/48.

La respuesta conserva `items`, `total`, `page`, `pageSize` y agrega `indicadores`:
total, pendientes, confirmadas, expiradas, canceladas, anuladas, con devolución,
no presentadas/completadas históricas y saldo vigente.

Los indicadores se agregan en SQL sobre todo el filtro **antes de paginar**. No se cuentan
los elementos de la página ni se materializa todo el histórico para calcularlos.
La consulta usa una cabecera de Pago por reserva y evita multiplicar dinero por detalle horario.

Los items incluyen importe, cobrado, devuelto, saldo vigente, fecha local, anulación,
responsable/fecha del último reembolso identificable y observación de evento de reserva.
Los datos de horarios y reembolsos se consultan en bloque solo para los IDs de la página.
Las reservas canceladas/expiradas/anuladas no generan saldo vigente por cobrar.

No se atribuye una anulación o cancelación a `UserNameUpdate`. Sin evento específico se
indica esa ausencia. Un reembolso histórico sin movimiento no recibe un autor/fecha inventados.

## Detalle desde el código de reserva

`GET /api/Reserva/gestion/{id}` reutiliza el detalle existente, con verificación del alcance
del usuario y lectura de anuladas. Estas se identifican mediante `anulada=true`, sin activar
acciones de pago. El endpoint anterior de detalle no cambia su comportamiento para otros clientes.

## Pantalla

- Inicio con Hoy y una sola búsqueda tras cargar las canchas/estados autorizados.
- Atajos Hoy, Esta semana y Este mes conservan los demás filtros y reinician página 1.
- Buscar y Limpiar; Limpiar vuelve al período de hoy, igual que al entrar al listado.
- Indicadores sobre el conjunto completo y tabla paginada con código enlazado, estados, importes y devolución.
- La tabla comparte el componente visual de reportes, sin badges en Estado/Pago ni columna Ver.
- Se cancela la solicitud anterior al cambiar de búsqueda para evitar sobrescrituras tardías.

## Verificación

```powershell
dotnet test Reserva.sln --configuration Release
```

Desde gestión:

```powershell
npm run build:development
npm test -- --watch=false --browsers=ChromeHeadless --include="src/app/features/reporte/**/*.spec.ts" --include="src/app/features/reserva/**/*.spec.ts"
```

La cobertura incluye agregado completo frente a página, cabecera opcional, anuladas,
fecha local, alcance frente a IDs enviados por el cliente, detalle de gestión para ambos
roles, Excel sin reservas y traducción de consultas en SQL Server/MySQL sin abrir conexión.
