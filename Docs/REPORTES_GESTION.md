# Reportes de gestión

## Alcance acordado

Se implementan tres reportes, consultables en pantalla y descargables como `.xlsx`.
Los dos primeros son del proveedor y el tercero es exclusivo del operador.

| Reporte | Objetivo | Criterio |
|---|---|---|
| Control económico de reservas | Revisar importes, cobros acumulados, devoluciones y saldos | Fecha local de la reserva; situación económica actual |
| Demanda de canchas y horarios | Identificar canchas, días y franjas con más o menos reservas | Horas confirmadas; pendientes y canceladas por separado |
| Agenda y cobros del día | Atender al cliente y conocer el saldo por cobrar | Un día seleccionable, hoy por defecto; canchas asignadas |

### Control económico

- Filtros: desde/hasta, cancha, estado de reserva y estado de pago.
- Total contratado vigente: suma de importes de reservas pendientes/confirmadas activas.
- Cobrado acumulado: importe registrado como recibido en el pago de cada reserva.
- Devuelto: `MontoReembolso` del pago.
- Cobrado neto: cobrado acumulado menos devuelto, incluyendo dinero retenido en cancelaciones.
- Saldo vigente: máximo entre cero y total menos cobrado, solo para reservas vigentes.
- Una fila por reserva: los detalles horarios no multiplican importes.
- Los importes ya corregidos se toman del acumulado actual; no se suman correcciones como cobros nuevos.

Este reporte no es una caja por fecha de cobro ni un estado de resultados/utilidad.
Una reserva del viernes con adelanto del lunes figura en el viernes. También puede
incluir reservas antiguas sin movimientos detallados.

### Demanda

- Filtros: rango de fechas y cancha.
- Agrupación: cancha + día de semana + hora inicial/final de la franja.
- Resumen de reservas confirmadas distintas y horas confirmadas reales.
- Reservas/horas pendientes, cancelaciones y expiraciones se muestran separadas.
- La duración se calcula entre inicio/fin: media hora es 0.5 h y 23:00–01:00 son 2 h.
- Una reserva con varios detalles se cuenta una sola vez en el resumen global.
- Las franjas configuradas actualmente sin reservas aparecen con cero demanda.
- Los horarios de reservas históricas se incluyen aunque la franja ya no esté activa.

No se presenta porcentaje de ocupación histórica ni asistencia: el sistema no tiene
un historial de capacidad/configuración ni el registro de llegada implementado.

### Agenda del operador

- Acceso exclusivamente con rol Operador, tanto en la ruta de pantalla como en la API/Excel.
- Canchas asignadas actualmente al operador, con asignación activa y del mismo proveedor.
- Incluye las reservas de esas canchas aunque fueran creadas por web, proveedor u otro operador.
- Por defecto se muestran pendientes y confirmadas del día seleccionado.
- Campos: horario agrupado, cancha, código, cliente, teléfono, estados, total, cobrado,
  saldo vigente y observaciones. En pantalla el teléfono permite iniciar una llamada.
- Indicadores: reservas vigentes, pendientes de confirmar, reservas con saldo y total por cobrar.

## Fechas e historial

- Desde/hasta son fechas de calendario inclusivas en la zona horaria de cada cancha.
- Las consultas SQL aplican alcance y límites temporales antes de materializar los datos.
- Las canchas inactivas siguen disponibles para el histórico del proveedor.
- Una reserva anulada por la cancelación simple (`Activo=false`) se trata como cancelada,
  no como una pre-reserva pendiente de cobro.
- Los estados de reservas/pagos son los actuales al generar el reporte.

## API

Controlador: `Reserva.Api/Controllers/Reporte/ReporteController.cs`.

| Método | Ruta | Rol |
|---|---|---|
| GET | `/api/Reporte/filtros` | Proveedor / Operador |
| POST | `/api/Reporte/economico` | Proveedor |
| POST | `/api/Reporte/economico/excel` | Proveedor |
| POST | `/api/Reporte/demanda` | Proveedor |
| POST | `/api/Reporte/demanda/excel` | Proveedor |
| POST | `/api/Reporte/agenda-operador` | Operador |
| POST | `/api/Reporte/agenda-operador/excel` | Operador |

```json
{
  "fechaDesde": "2026-11-01",
  "fechaHasta": "2026-11-13",
  "idCancha": null,
  "estadoReserva": null,
  "estadoPago": null
}
```

La agenda exige fechas iguales; admite `estadoReserva: "VIGENTES"` para pendientes y
confirmadas. Demanda no admite filtros de estado, pues compara todos por separado.
Los códigos y nombres válidos para los filtros proceden de `/filtros`.

No se aceptan identificadores de proveedor/operador desde el cliente. El alcance sale
del JWT autenticado y de las asignaciones persistidas. Una cancha fuera del alcance
se rechaza incluso cuando el identificador se modifica directamente en la petición.

## Excel y rendimiento

- Generación con ClosedXML existente en `Reserva.Common`, mediante `Excel/ExportadorExcel.cs`.
- Hojas **Resumen** (filtros, criterio, fecha de generación e indicadores) y **Detalle**.
- Importes numéricos, fechas de Excel y teléfonos como texto para conservar ceros iniciales.
- Los textos no se escriben como fórmulas.
- El Excel vuelve a consultar con los filtros aplicados y exporta todas las filas, no
  solo la página visible. Cada archivo tiene su propia fecha de generación.
- Máximo de 366 días, 25 000 reservas/filas, 25 000 franjas configuradas y 250 000 detalles por consulta. Al superar el límite
  se solicita reducir el rango; no se genera un archivo truncado.
- Pantalla paginada a 20 filas. Al cambiar filtros es necesario Consultar antes de exportar.
- No se agregan tablas ni cambios de esquema para estos reportes.

## Pantallas y permisos

```text
Reportes
├── Control económico        [Proveedor]
├── Demanda de horarios      [Proveedor]
└── Agenda y cobros del día   [Operador]
```

Las rutas bajo `/admin/reportes` son lazy-loaded y usan roles estrictos. El bypass
habitual del rol Admin no habilita estos reportes exclusivos sin el rol requerido.
El menú filtra también los subítems y elimina grupos sin hijos autorizados.

## Pruebas

```powershell
dotnet test Reserva.sln --configuration Release
```

Desde `ReservaCanchas_admin_Web`:

```powershell
npm run build:development
npm test -- --watch=false --browsers=ChromeHeadless --include="src/app/features/reporte/**/*.spec.ts" --include="src/app/shared/layouts/dashboard-layout/core/utils/menu.utils.spec.ts" --include="src/app/core/auth/guards/role.guard.spec.ts"
```

La cobertura incluye fórmulas económicas, separación de estados, medias horas y
medianoche, alcance de proveedor/operador, filtros, paginación/exportación completa,
archivo Excel real y traducción de consultas para SQL Server/MySQL sin conexión a BD.
