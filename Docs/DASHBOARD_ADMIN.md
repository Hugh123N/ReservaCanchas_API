# Dashboard del administrador

## Acceso

- `GET /api/Dashboard/admin?meses=6` (también admite `12`).
- Requiere JWT Bearer y rol `Admin`; el handler vuelve a comprobar el rol antes de consultar repositorios.
- Otros períodos devuelven un `ResponseDto` inválido. El token de cancelación se propaga hasta EF Core.
- Vista Angular: `/admin/dashboard/administrador`.

## Indicadores y reglas

### Cobros

Se consultan exclusivamente `PagoPlan` activos, positivos, en PEN, con fecha de pago no futura y estado identificado por `EstadoPago.Codigo = Constants.ESTADO_PAGO.Pagado`.
No se usan importes de reservas, precios de catálogo multiplicados por suscriptores ni pagos pendientes/rechazados.

Los cobros no dependen de que el proveedor o su suscripción continúen activos: los pagos de planes anteriores permanecen en el histórico.
Si se repite un `CulqiChargeId`, se cuenta solo el primer registro confirmado válido por `IdPagoPlan`, incluso si las copias pertenecen a otro mes.
Pagos sin identificador de cargo se consideran operaciones distintas.

El importe es cobrado bruto, antes de comisiones de pasarela. No es utilidad ni un cálculo de liquidación neta.
Se usa la fecha registrada en `PagoPlan.FechaPago`; el dashboard no consulta Culqi ni modifica el procesamiento de pagos.

La comparación del mes actual es contra el mismo tramo del anterior en horario de Perú. Se conserva la hora y se ajusta el día al último existente si el mes anterior es más corto.
Sin cobros positivos en la base comparable, la variación porcentual es `null` y se muestra «Sin base de comparación».

### Proveedores y planes

- Registrados: todas las filas de proveedor, con desglose por `Activo` e incorporaciones del mes.
- Los gráficos de registros incluyen proveedores luego inactivados.
- Para planes y pendientes de planes solo se consideran proveedores activos y registros `ProveedorPlan.Activo && EsActual`.
- Si hay varios registros actuales del mismo proveedor, se elige el mayor `IdProveedorPlan` para evitar duplicar proveedores.
- Plan de pago vigente: `ACTIVE`, no prueba gratuita, tarifa de precio positivo y `FechaInicio <= ahora < FechaFin`.
- Prueba vigente: `EsPruebaGratis`, `ACTIVE` y las mismas condiciones de vigencia.
- La cancelación programada no quita la vigencia antes de finalizar el período.
- Gracia: estados `GRACE` o `PAST_DUE`, incluso si el proceso de expiración todavía no ha actualizado un registro atrasado.
- Suspendidos: estado `SUSPENDED`.

La distribución por plan usa solamente proveedores con plan de pago vigente. Las pruebas se presentan aparte.
El catálogo de planes/tarifas puede estar inactivo sin borrar la contratación histórica.

### Canchas

Canchas con `Activo = true`, proveedor activo y estado `Aprobado` para el indicador principal, o estado `Pendiente` para revisión.
El booleano `Activo` no equivale a aprobación. No existe una aprobación de proveedores en el dashboard: estos se registran activos y su inactivación corresponde a gestión.

## Evolución y atención

- Se entregan 6 o 12 meses, incluido el actual y rellenando meses sin movimientos con cero.
- Los límites se construyen en horario de Perú (UTC-5) y se comparan en UTC.
- Los agregados se ejecutan en SQL; no se cargan todos los pagos, proveedores o canchas en memoria.
- El cambio de período aplica a los gráficos históricos, no convierte los indicadores actuales en históricos.
- Pendientes: canchas por revisar; pruebas que terminan en 7 días; gracia; suspendidos; vencimientos en 7 días sin autorrenovación; cancelaciones programadas aún vigentes.
- Vencimientos sin autorrenovación excluye cancelaciones programadas para no duplicarlas en esa categoría.
- Cada categoría devuelve su cantidad total y hasta 5 casos prioritarios. Canchas: solicitudes más antiguas; planes: fecha relevante más próxima.
- Los casos muestran nombres, cancha/plan y fecha, sin IDs internos. Las categorías no se suman como si fueran proveedores únicos: pueden solaparse.
- La aprobación de canchas se realizará en la pantalla de gestión, no en este dashboard de lectura.

## Verificación

`DashboardAdminTests` cubre deduplicación de cobros, estados, monedas, horario de Perú, ceros, períodos, planes de prueba, vigencias, proveedores inactivos y traducción SQL Server/MySQL sin conectarse a bases reales.
`GestionReservaAuthenticationTests.DashboardAdmin_ExigeBearerYRolAdmin` verifica la autorización con la configuración JWT real de la API.
