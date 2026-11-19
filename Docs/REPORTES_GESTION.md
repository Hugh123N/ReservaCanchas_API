# Reportes de gestión de proveedor y operador

Documento único de referencia funcional y técnica de los reportes implementados para **Proveedor** y **Operador**. Incluye el modelo de cobros que los alimenta y la consulta de Reservas relacionada con ellos.

## Contenido

1. [Mapa de reportes y preguntas principales](#1-mapa-de-reportes-y-preguntas-principales)
2. [Permisos, fechas y reglas comunes](#2-permisos-fechas-y-reglas-comunes)
3. [Control económico de reservas](#3-control-económico-de-reservas)
4. [Demanda de canchas y horarios](#4-demanda-de-canchas-y-horarios)
5. [Control de movimientos y responsables](#5-control-de-movimientos-y-responsables)
6. [Agenda y cobros del día del operador](#6-agenda-y-cobros-del-día-del-operador)
7. [Consulta complementaria de Reservas](#7-consulta-complementaria-de-reservas)
8. [Modelo de pagos y operaciones que alimentan los reportes](#8-modelo-de-pagos-y-operaciones-que-alimentan-los-reportes)
9. [API y contratos](#9-api-y-contratos)
10. [Implementación técnica, pantalla y Excel](#10-implementación-técnica-pantalla-y-excel)
11. [Verificación y preparación de datos](#11-verificación-y-preparación-de-datos)

## 1. Mapa de reportes y preguntas principales

| Reporte | Rol | Finalidad | Pregunta principal | Fecha de referencia |
|---|---|---|---|---|
| Control económico de reservas | Proveedor | Conocer la situación económica actual de las reservas de un período. | ¿Cuánto está contratado, cobrado, devuelto y por cobrar de esas reservas? | Fecha local de la reserva. |
| Demanda de canchas y horarios | Proveedor | Identificar qué canchas, días y franjas concentran las reservas. | ¿Qué horarios tienen más horas confirmadas y cuáles muestran poca demanda? | Fecha local de la reserva y sus bloques horarios. |
| Control de movimientos y responsables | Proveedor | Supervisar las operaciones registradas por persona, cancha y método. | ¿Quién registró cobros, devoluciones y correcciones durante el período? | Fecha local de registro de la operación. |
| Agenda y cobros del día | Operador | Organizar la atención y los cobros de sus canchas asignadas. | ¿A quién atiendo hoy y cuánto queda por cobrar? | Un día de reserva seleccionado. |

Los cuatro reportes se consultan en pantalla y se descargan como **Excel `.xlsx`**.

El listado **Reservas** es una consulta operativa complementaria, disponible para ambos roles. Allí quedó la situación de reservas que anteriormente aparecía dentro del reporte de movimientos; no constituye un quinto reporte ni genera una hoja adicional en ese Excel.

### Elegir el reporte correcto

- Para revisar el dinero asociado a reservas de determinados días: **Control económico**.
- Para analizar canchas y franjas horarias: **Demanda**.
- Para revisar operaciones realizadas en determinados días: **Movimientos y responsables**.
- Para atender las reservas de un día como operador: **Agenda y cobros del día**.
- Para localizar una reserva, abrir su detalle o revisar su cancelación/devolución: **Reservas**.

**Ejemplo:** una reserva para el viernes recibe S/ 50 el lunes. En Control económico esos S/ 50 aparecen al consultar el viernes; en Movimientos aparecen al consultar el lunes. Ambos resultados son correctos porque usan fechas diferentes.

## 2. Permisos, fechas y reglas comunes

### Alcance del usuario

- El proveedor consulta las canchas de su negocio, incluidas las inactivas para conservar el histórico.
- El operador consulta únicamente sus asignaciones activas, de su proveedor y con vínculo válido al usuario autenticado.
- La agenda incluye reservas de esas canchas aunque las haya creado la web pública, el proveedor u otro operador.
- La autorización procede del JWT y de las relaciones persistidas. Un identificador de proveedor u operador enviado por el cliente no amplía el alcance.
- Una cancha solicitada fuera del alcance se rechaza. La API y el Excel aplican las mismas restricciones.
- Las rutas de reportes del frontend usan roles estrictos y el menú filtra también los subítems.

### Fechas y estados

- Los filtros Desde/Hasta son fechas de calendario **inclusivas**, interpretadas en la zona horaria de cada cancha.
- El backend calcula los límites UTC para consultar SQL y convierte los resultados a la zona de la cancha. El extremo superior SQL es exclusivo al día siguiente.
- La fecha de operación de un movimiento es `MovimientoPago.CreateDate`, persistida en UTC; no es la fecha de juego ni una fecha original inferida.
- `GeneradoEn` identifica cuándo se produjo el resultado. El Excel tiene su propia generación porque vuelve a consultar los datos.
- Los estados y acumulados de la reserva/pago son los actuales al consultar; no representan una fotografía histórica del estado que tenían aquel día.
- Una reserva es **vigente** si está activa y Pendiente o Confirmada.
- Una reserva anulada mediante `Activo = false` se presenta como cancelada, no como pendiente de cobro.
- Las reservas canceladas, expiradas o anuladas no generan importe **Por cobrar**.

Los códigos del catálogo vigente se obtienen desde la API de filtros y se comparan mediante `Constants`:

| Catálogo | Códigos |
|---|---|
| Reserva | `01` Pendiente, `02` Confirmado, `03` Cancelado, `04` Expirado; `05` No presentado y `06` Completado se conservan para información histórica. |
| Pago | `01` Pagado, `02` Parcial, `03` Pendiente, `04` Cancelado, `05` Rechazado. |

## 3. Control económico de reservas

**Rol:** Proveedor. **Pantalla:** `/admin/reportes/economico`.

### Finalidad

Revisar la situación económica actual de las reservas correspondientes al período de juego seleccionado: importes, dinero registrado, devoluciones y saldo por cobrar.

### Preguntas que responde

- ¿Cuántas reservas hay en el período consultado?
- ¿Cuál es el importe contratado de las reservas que siguen vigentes?
- ¿Cuánto se ha registrado como cobrado en esas reservas?
- ¿Cuánto se ha devuelto y cuánto queda retenido después de devoluciones?
- ¿Cuánto falta cobrar de las reservas vigentes?
- ¿Qué reserva y cancha explican cada importe?

### Filtros, indicadores y detalle

Filtros: Desde/Hasta, cancha, estado de reserva y estado de pago. Al entrar se consulta desde el primer día del mes hasta hoy.

| Indicador | Cálculo implementado |
|---|---|
| Reservas | Cantidad de reservas resultantes del filtro. |
| Importe vigente | Suma del total de las reservas activas pendientes/confirmadas. |
| Cobrado acumulado | Suma de `Pago.MontoAdelanto`. Sin Pago, cero. |
| Devuelto | Suma de `Pago.MontoReembolso`, con nulos tratados como cero. |
| Cobrado neto | Cobrado acumulado menos devuelto. Incluye dinero retenido de reservas canceladas. |
| Por cobrar | Para cada reserva vigente: `max(0, total - cobrado)`; para las demás, cero. |

El total se obtiene de `Pago.Monto` cuando existe cabecera activa; en caso contrario se conserva `Reserva.MontoTotal`.

El detalle tiene **una fila por reserva**: fecha, código, cancha, horarios agrupados, cliente, estado de reserva/pago, importe, cobrado acumulado, devuelto, neto y por cobrar. Los bloques horarios no multiplican los importes.

### Implementación e interpretación

`ReporteService` consulta reservas y detalles dentro del alcance y período autorizados. `ReporteBuilder.ConstruirEconomicoAgenda` construye filas e indicadores con los acumulados actuales de Pago.

- Una corrección ya aplicada se refleja en el acumulado; no se suma otra vez como cobro.
- Las reservas antiguas sin movimientos detallados pueden aparecer porque el reporte usa la cabecera actual.
- Este resultado no es caja por fecha de cobro ni utilidad: no incorpora gastos y no atribuye los cobros al día en que se registraron.

## 4. Demanda de canchas y horarios

**Rol:** Proveedor. **Pantalla:** `/admin/reportes/demanda`.

### Finalidad

Reconocer la demanda registrada por cancha, día de semana y franja horaria para apoyar decisiones sobre horarios y oferta.

### Preguntas que responde

- ¿Qué canchas acumulan más horas confirmadas?
- ¿Qué días y franjas tienen mayor o menor demanda?
- ¿Qué franjas configuradas no registran reservas en el período?
- ¿Cuántas reservas y horas siguen pendientes de confirmar?
- ¿En qué franjas aparecen cancelaciones o expiraciones?

### Filtros, indicadores y detalle

Filtros: Desde/Hasta y cancha. Al entrar se consulta desde el primer día del mes hasta hoy. No admite filtros de estado porque presenta los estados por separado.

- Indicadores: reservas confirmadas, horas confirmadas, pendientes y canceladas.
- Agrupación: **cancha + día de semana + hora inicial/final de la franja**.
- Por grupo: reservas/horas confirmadas, reservas/horas pendientes, canceladas y expiradas.
- Orden principal: mayor cantidad de horas confirmadas.

### Implementación e interpretación

`ReporteConsultas` obtiene los detalles de reservas y las franjas actualmente configuradas; `ReporteBuilder.ConstruirDemanda` forma los grupos.

- Se deduplican detalles por reserva y horario.
- La duración se calcula entre inicio y fin: media hora equivale a `0.5 h`; `23:00–01:00` equivale a `2 h`.
- Las franjas activas configuradas cuyo día de semana aparece en el rango se incorporan aunque su demanda sea cero.
- Los detalles históricos se incluyen aunque su franja ya no esté activa.
- Una reserva con varios bloques se cuenta una sola vez en el indicador global. Puede aparecer en varias franjas; sumar los conteos de todas las filas no equivale a contar reservas únicas globales.
- Las anuladas se consideran canceladas.

El reporte mide **horas reservadas confirmadas**, no asistencia real ni porcentaje de ocupación histórica. No existe un historial completo de capacidad/configuración ni un registro de llegada que permita sostener esos indicadores.

## 5. Control de movimientos y responsables

**Rol:** Proveedor. **Pantalla:** `/admin/reportes/control-movimientos`.

### Finalidad

Supervisar lo que se registró durante un período y quién lo hizo, separando movimientos de dinero, ajustes de registro e incorporación de datos históricos.

### Preguntas que responde

- ¿Cuánto se registró como cobrado y reembolsado durante el período?
- ¿Qué neto resulta de esas entradas y salidas?
- ¿Qué responsable registró cada operación y en qué cancha?
- ¿Por qué método se registraron los cobros y devoluciones?
- ¿Qué correcciones se hicieron, por qué importe y con qué motivo?
- ¿A qué reserva corresponde cada operación, aunque se juegue otro día?

### Filtros y comportamiento

- Desde/Hasta por **fecha de operación**, cancha y responsable.
- Filtros adicionales: método, tipo (`COBRO`, `REEMBOLSO`, `CORRECCION`) y código completo o parcial de reserva.
- Responsables: proveedor, operadores actuales/inactivos y usuarios históricos vinculados a movimientos de sus canchas; también existe Históricos sin identificar.
- Inicio con Hoy. Los botones **Hoy**, **Esta semana** y **Este mes** aplican fechas y consultan inmediatamente, conservando los demás filtros.
- Aquí Esta semana significa **lunes hasta hoy** y Este mes significa **primer día del mes hasta hoy**.
- Los cambios manuales de filtros requieren Consultar antes de exportar.

### Indicadores y tablas

| Indicador | Cálculo |
|---|---|
| Cobrado registrado | Suma de importes `COBRO` filtrados. |
| Reembolsado | Suma de importes `REEMBOLSO` filtrados. |
| Neto de movimientos | Cobros menos reembolsos; puede ser negativo. |
| Reservas con movimientos | Reservas distintas con `COBRO`, `REEMBOLSO` o `CORRECCION`. |
| Correcciones realizadas | Cantidad de correcciones filtradas, en su apartado. |
| Ajuste registrado | Suma firmada de las correcciones, en su apartado. |

La pantalla contiene:

1. **Resumen por responsable y cancha:** cobros, reembolsos, neto, cantidad de operaciones, reservas distintas y correcciones/ajuste separados.
2. **Desglose por método, responsable y cancha:** únicamente cobros y reembolsos.
3. **Detalle de cobros y reembolsos:** fecha/hora de operación, responsable, rol, cancha, reserva, fecha de reserva, acción, importe, método y referencia/motivo.
4. **Correcciones de registro:** ajuste firmado, acumulado cobrado anterior/resultante y motivo.
5. **Saldos históricos incorporados:** importes y observaciones no atribuibles a cobros nuevos del período.

Los apartados secundarios son desplegables. Las tablas y el Excel muestran nombres, sin columnas de IDs internos del responsable. La identidad y agrupación se mantienen por GUID internamente para separar personas con el mismo nombre. El código de reserva del detalle enlaza a la reserva.

### Implementación e interpretación

`ControlMovimientosService` obtiene el alcance, valida el método, aplica el período en SQL y localiza fechas. `MovimientosReporteConsultas` proyecta cada `MovimientoPago`. `ControlMovimientosBuilder` genera las tablas y sus indicadores.

- El método pertenece a cada operación, no a la cabecera de pago.
- Una corrección es un ajuste global del acumulado. No es dinero nuevo, una devolución ni prueba de quién cometió el error original.
- Filtrar un método no atribuye a ese medio las correcciones globales: cuando el filtro de tipo las incluye, se conservan separadas. No se acepta combinar tipo `CORRECCION` con un método concreto.
- `SALDO_INICIAL` identifica la incorporación de un acumulado antiguo. No entra en cobros, devoluciones ni neto, ni se atribuye a un responsable del período.
- Un filtro por persona excluye los saldos históricos; una persona concreta y Solo sin responsable no pueden solicitarse a la vez.
- En `COBRO`, `CORRECCION` y `SALDO_INICIAL`, anterior/resultante corresponden a lo **cobrado**; en `REEMBOLSO`, a lo **devuelto**.
- Un reembolso de dinero cobrado antes puede producir neto negativo en el período actual.

El neto no representa utilidad, entrega de caja ni conciliación bancaria. El Excel tiene cuatro hojas: **Resumen**, **Responsables**, **Metodos** y **Movimientos**. Esta última separa entrada monetaria, salida monetaria, ajuste e histórico; no incluye una hoja de situación de reservas.

## 6. Agenda y cobros del día del operador

**Rol:** Operador. **Pantalla:** `/admin/reportes/agenda-operador`.

### Finalidad

Concentrar la información necesaria para atender las reservas de un día y conocer los importes pendientes de las canchas asignadas al operador.

### Preguntas que responde

- ¿Qué reservas debo atender el día seleccionado y en qué horarios?
- ¿En qué cancha se juega y quién es el cliente?
- ¿Qué reservas siguen pendientes de confirmar?
- ¿Cuánto está cobrado y cuánto falta cobrar?
- ¿Cómo contacto al cliente y qué observaciones debo considerar?

### Filtros, indicadores y detalle

- Un día seleccionable, hoy por defecto; cancha, estado de reserva y estado de pago.
- Al entrar se seleccionan reservas **VIGENTES**: pendientes y confirmadas activas.
- Indicadores: reservas vigentes, pendientes de confirmar, reservas con saldo y total por cobrar.
- Detalle: horario agrupado, cancha, código, cliente, teléfono, estados, total, cobrado, por cobrar y observaciones.
- El teléfono permite iniciar una llamada desde la pantalla.

### Implementación e interpretación

Comparte `ReporteService` y el cálculo económico de `ReporteBuilder`, con columnas e indicadores específicos para agenda. `GetReporteQueryValidator` exige `FechaDesde == FechaHasta`.

El resultado respeta las asignaciones actuales del operador y muestra las reservas de esas canchas independientemente de quién las creó. Es una consulta de atención; registrar cobros se realiza mediante las operaciones de la reserva y su pago.

## 7. Consulta complementaria de Reservas

**Roles:** Proveedor y Operador. **Pantalla:** `/admin/reservas`.

### Finalidad y preguntas que responde

Localizar reservas y revisar su situación actual dentro del alcance del usuario:

- ¿Dónde está una reserva por código, cliente o teléfono?
- ¿Cuántas están pendientes, confirmadas, expiradas o canceladas con estos filtros?
- ¿Cuánto hay por cobrar sobre todo el conjunto consultado?
- ¿La reserva fue anulada o tiene devolución?
- ¿Quién registró el último reembolso identificable y cuándo?

### Comportamiento actual

- Filtros de fechas locales, cancha, estados y texto. Se admiten `VIGENTES` y `ANULADAS` como selecciones especiales.
- Inicio con Hoy. Limpiar restaura Hoy y borra los otros filtros.
- Los atajos conservan los demás filtros, consultan y reinician la página. **En Reservas, Esta semana abarca lunes a domingo completo**, a diferencia del reporte de movimientos.
- La pantalla ofrece 12/24/48 filas. La API admite páginas de hasta 200.
- Las seis tarjetas visibles son: Reservas consultadas, Pendientes, Confirmadas, Expiradas, Canceladas (incluye anuladas) y Por cobrar.
- El código enlaza al detalle; no hay columna Ver/ojito.
- Incluye fecha local, cancha, horarios, cliente, estados, total, cobrado, devuelto, por cobrar, evento de reserva y último reembolso.
- Una búsqueda nueva cancela la anterior para evitar que una respuesta antigua sobrescriba el resultado reciente.

### Implementación e interpretación

`SearchReservaConsultas` aplica alcance, período y filtros, une una cabecera activa de Pago por reserva y calcula los indicadores en SQL **antes de paginar**. La respuesta conserva `items`, `total`, `page`, `pageSize` y agrega `indicadores`.

El contrato incluye también cantidades de anuladas, reservas con devolución y estados históricos de no presentada/completada. Anuladas es un subconjunto de canceladas, no una categoría que deba sumarse otra vez.

`SearchReservaPaginaMapper` completa horarios y último reembolso mediante consultas en bloque solo para los IDs de la página. No se inventan responsables/fechas de reembolsos históricos ni se interpreta `UserNameUpdate` como autor de una cancelación. Cuando no existe un evento específico, se informa esa ausencia.

El detalle de gestión permite leer anuladas sin activar acciones de pago. Esta consulta no vuelve a introducir situación de reservas en el reporte de movimientos.

## 8. Modelo de pagos y operaciones que alimentan los reportes

### Entidades y significado

| Fuente | Responsabilidad |
|---|---|
| `Reserva` | Identidad, cancha, cliente, fecha de juego, estado e importe cuando todavía no existe Pago. |
| `DetalleReserva` / `HorarioCancha` | Bloques horarios, precios, duración y configuración utilizados para agenda y demanda. |
| `Pago` | Cabecera opcional, como máximo una por reserva: total, cobrado acumulado, pendiente, devuelto, estado, moneda y revisión. |
| `MovimientoPago` | Detalle de operación: tipo, importe, acumulados anterior/resultante, método, referencia, responsable, rol y auditoría. |

Una reserva sin dinero recibido conserva el precio en `Reserva.MontoTotal`, sin crear una cabecera Pago ni movimientos vacíos. Consultar su historial no escribe datos: devuelve `idPago: 0`, importe, saldo, permisos e historial vacío.

Los nuevos movimientos obtienen `IdUsuarioResponsable` y `RolResponsable` del JWT. Los nulos se reservan para información histórica no identificable. No se deducen identidades comparando nombres ni existen los campos adicionales `IdUsuarioRegistro`, `FechaOperacion` o `IdMovimientoOrigen` en este modelo.

### Flujos implementados

| Operación | Efecto sobre el registro |
|---|---|
| Crear reserva normal sin dinero recibido | Reserva pendiente, sin Pago ni movimientos. |
| Crear reserva inmediata con cobro | Crea Pago y un `COBRO` con usuario, método y referencia. |
| Registrar primer cobro posterior | Crea la cabecera y su primer `COBRO` dentro de la transacción. |
| Registrar otro abono | Añade `COBRO`; actualiza acumulados, estado y revisión. |
| Editar observaciones/horarios | Conserva lo cobrado y recalcula importe/saldo con precios del servidor; no genera un cobro ficticio. |
| Corregir registro | Solo proveedor; guarda acumulado anterior/correcto, diferencia firmada y motivo. No representa dinero recibido o devuelto. |
| Cancelar con devolución inmediata | Valida permisos, política, revisión e importe; registra `REEMBOLSO`, incrementa devuelto, cierra el pendiente y cancela la reserva. |

Los métodos presenciales admitidos son efectivo, transferencia, Yape y Plin. Estas operaciones registran dinero recibido/devuelto fuera de la plataforma; no ejecutan cargos ni transferencias por una pasarela.

### Validaciones y consistencia

- El operador puede cobrar en reservas de hoy/futuras de sus canchas asignadas; el proveedor puede operar también sobre reservas anteriores que sigan vigentes.
- Una corrección exige Pago existente y motivo de al menos cinco caracteres. No puede reducir el acumulado por debajo de lo ya reembolsado.
- Se rechazan cobros/correcciones de reservas canceladas, expiradas o pagos cancelados.
- Los cobros son positivos, respetan el saldo disponible y admiten hasta dos decimales; una corrección puede tener diferencia negativa.
- El adelanto mínimo se valida al crear una reserva inmediata. Cada abono posterior no necesita alcanzar de nuevo ese porcentaje; alcanzar el mínimo puede confirmar una reserva pendiente.
- Una corrección refleja el acumulado correcto aunque quede por debajo del mínimo; no cancela ni desconfirma automáticamente la reserva.
- Si editar horarios reduce el precio por debajo de lo recibido, primero debe resolverse la devolución. Una reserva con cobros no puede anularse mediante cancelación simple.
- `idOperacion` permite reintentar una operación confirmada sin duplicar dinero. Reutilizarla con otros datos se rechaza.
- `revision` de Pago y los controles concurrentes de Reserva evitan sobrescribir cambios. Una revisión desactualizada exige recargar el detalle.
- Después del primer cobro se puede consultar por reserva para obtener el ID definitivo de Pago tras el commit.
- Las rutas genéricas de crear/actualizar/eliminar Pago y las anteriores de confirmar/completar no permiten modificar pagos de reservas; se utilizan las operaciones de movimientos.

### Información histórica

Si existe un acumulado antiguo sin movimientos, la primera operación puede incorporar un `SALDO_INICIAL`. La fecha corresponde a la incorporación al historial, no al cobro original, y no se inventan responsable ni método.

Los acumulados y devoluciones históricas se conservan en los resúmenes. No se fabrican desgloses de sus personas/métodos. Los archivos técnicos de legado, si una instalación los conserva, no son fuentes operativas de los reportes.

## 9. API y contratos

### Reportes

Controlador: `Reserva.Api/Controllers/Reporte/ReporteController.cs`. Los endpoints autentican explícitamente con Bearer.

| Método | Ruta | Rol |
|---|---|---|
| GET | `/api/Reporte/filtros` | Proveedor / Operador |
| POST | `/api/Reporte/economico` | Proveedor |
| POST | `/api/Reporte/economico/excel` | Proveedor |
| POST | `/api/Reporte/demanda` | Proveedor |
| POST | `/api/Reporte/demanda/excel` | Proveedor |
| POST | `/api/Reporte/agenda-operador` | Operador |
| POST | `/api/Reporte/agenda-operador/excel` | Operador |
| GET | `/api/Reporte/control-movimientos/filtros` | Proveedor |
| POST | `/api/Reporte/control-movimientos` | Proveedor |
| POST | `/api/Reporte/control-movimientos/excel` | Proveedor |

Contrato de económico, demanda y agenda:

```json
{
  "fechaDesde": "2026-11-01",
  "fechaHasta": "2026-11-17",
  "idCancha": null,
  "estadoReserva": null,
  "estadoPago": null
}
```

Para agenda se envía el mismo día en ambas fechas; `estadoReserva: "VIGENTES"` selecciona pendientes y confirmadas. Demanda exige estados vacíos/nulos.

Contrato de movimientos:

```json
{
  "movimientos": {
    "fechaDesde": "2026-11-01",
    "fechaHasta": "2026-11-17",
    "idCancha": null,
    "idUsuarioResponsable": null,
    "soloSinResponsable": false,
    "idMetodoPago": null,
    "tipo": null,
    "codigoReserva": null
  }
}
```

El código de reserva admite hasta 50 caracteres. El catálogo de responsables históricos se limita a 10 000 entradas; si se supera, la API informa el exceso.

Las consultas devuelven `ResponseDto<T>`. Los datos tabulares usan `ReporteDto`: título, criterio, período, cancha, fecha de generación, indicadores, columnas con formato y filas. `ControlMovimientosDto` contiene sus tablas separadas; `DetalleExcel` es interno y no duplica filas en el JSON.

Los endpoints Excel devuelven el archivo cuando la respuesta es válida o `400 BadRequest` con los mensajes si no se puede exportar.

### Reservas y movimientos de pago

| Método | Ruta | Uso |
|---|---|---|
| POST | `/api/Reserva/search` | Consulta paginada e indicadores dentro del alcance de proveedor/operador. |
| GET | `/api/Reserva/gestion/{id}` | Detalle autorizado de gestión, incluidas anuladas. |
| GET | `/api/Pago/reserva/{idReserva}/movimientos` | Historial por reserva, exista o no cabecera. |
| GET | `/api/Pago/{idPago}/movimientos` | Historial de una cabecera existente. |
| POST | `/api/Pago/registrar-cobro` | Primer cobro o nuevo abono. |
| POST | `/api/Pago/corregir-registro` | Corrección del acumulado, exclusiva del proveedor. |
| POST | `/api/Reserva/liberar-reserva-operador` | Cancelación y eventual devolución inmediata. |

Ejemplo de búsqueda de Reservas:

```json
{
  "filter": {
    "fechaDesdeLocal": "2026-11-17",
    "fechaHastaLocal": "2026-11-17",
    "idCancha": null,
    "codigoEstado": null,
    "codigoEstadoPago": null,
    "searchText": null
  },
  "page": { "page": 1, "pageSize": 12 },
  "sort": [{ "property": "createDate", "direction": "desc" }]
}
```

Las fechas son opcionales en esta API; los campos anteriores `fechaDesde`/`fechaHasta` continúan aceptándose. El filtro anterior por nombre de cancha se conserva dentro del alcance y `idProveedor` se ignora como autorización.

Ejemplo de primer cobro:

```json
{
  "idPago": 0,
  "idReserva": 10,
  "idOperacion": "a9dc2907-e053-45c9-b679-28c552c9bc72",
  "revision": 0,
  "monto": 20,
  "codigoMetodoPago": "02",
  "referenciaOperacion": "REC-123",
  "motivo": null
}
```

Para un abono posterior se identifica la cabecera con `idPago`; si se envían ambos IDs deben corresponder a la misma reserva. En una corrección, `monto` es el **acumulado correcto**, no el importe adicional, y `motivo` es obligatorio. La devolución usa `revisionPago`, `idOperacion`, método y referencia en su contrato de liberación.

## 10. Implementación técnica, pantalla y Excel

### Capas y archivos principales

```text
Controller → Application → MediatR Query/Validator → Service/Consultas → Repository/EF Core
                                                              ↓
                                                        Builder → DTO
Application → ExportadorExcel → archivo .xlsx
```

Los siguientes paths son relativos a `ReservaCanchas_API`:

| Capa o responsabilidad | Implementación |
|---|---|
| Entrada y autorización de reportes | `Reserva.Api/Controllers/Reporte/ReporteController.cs` |
| Orquestación y exportación | `Reserva.Application/Reporte/ReporteApplication.cs` |
| Queries y validación | `Reserva.Domain/Queries/Reporte/` |
| Alcance del usuario | `Reserva.Domain/Services/Reporte/ReporteAccessService.cs` |
| Económico, demanda y agenda | `ReporteService.cs`, `ReporteConsultas.cs`, `ReporteBuilder.cs`, en `Reserva.Domain/Services/Reporte/` |
| Movimientos y responsables | `ControlMovimientosService.cs`, `MovimientosReporteConsultas.cs`, `ControlMovimientosBuilder.cs`, en el mismo directorio |
| Límites temporales y datos intermedios | `ReportePeriodo.cs`, `ReporteReservaData.cs`, `MovimientoReporteData.cs`, en el mismo directorio |
| Contratos de reportes | `Reserva.Dto/Reporte/` |
| Búsqueda e indicadores de Reservas | `Reserva.Domain/Queries/Dbo/Reserva/SearchReservaQueryHandler.cs`, `SearchReservaConsultas.cs`, `SearchReservaPaginaMapper.cs` |
| Operaciones de pago y sus permisos | `Reserva.Domain/Services/Pago/MovimientoPagoService.cs`, `PagoReservaAccessService.cs` |
| Generador Excel | `Reserva.Common/Excel/ExportadorExcel.cs` |

Los reportes consultan datos; no crean movimientos ni escriben pagos/reservas. Las escrituras pertenecen a los commands operativos y su Unit of Work.

### Pantalla

En `ReservaCanchas_admin_Web/src/app/features/reporte/`:

- `pages/reporte/`: pantalla compartida de económico, demanda y agenda.
- `pages/control-movimientos/`: filtros y apartados de movimientos/responsables.
- `components/reporte-resultado/`: indicadores, criterio, generación y paginación local de 20 filas.
- `components/reporte-tabla/`: columnas y formato compartidos, detalles desplegables y enlaces cuando corresponde.
- `core/services/reporte.service.ts`: consultas y descarga.

Los botones de exportación muestran icono de descarga y **Excel**. Se deshabilitan durante consulta/exportación o si hay filtros sin aplicar. El frontend conserva los filtros de la última consulta correcta para exportar ese mismo criterio.

La tabla de Reservas reutiliza la presentación tabular de reportes, con código enlazado y estados en texto.

### Excel

- Se utiliza ClosedXML, ya presente en `Reserva.Common`.
- Económico, demanda y agenda: hojas **Resumen** y **Detalle**.
- Movimientos: **Resumen**, **Responsables**, **Metodos** y **Movimientos**.
- Los importes y fechas se escriben como valores de Excel; teléfonos como texto para conservar ceros iniciales.
- Los textos no se escriben como fórmulas.
- La descarga consulta de nuevo en backend y contiene todas las filas autorizadas, no solo la página visible. Puede reflejar operaciones posteriores a la consulta de pantalla; incluye su fecha de generación.

### Límites y rendimiento

| Control | Límite actual |
|---|---|
| Período de reportes | Hasta 366 días inclusivos. |
| Reservas base de económico/demanda/agenda | 25 000 dentro del alcance/período, antes del filtrado adicional de estados. |
| Filas finales de esos reportes | 25 000. |
| Franjas configuradas consultadas para demanda | 25 000. |
| Detalles horarios consultados | 250 000. |
| Movimientos consultados | 50 000. |
| Catálogo de responsables históricos | 10 000 entradas. |

Se aplican alcance y límites temporales en SQL antes de materializar datos, mediante consultas sin seguimiento y proyecciones específicas. Se evita un `IN` con miles de reservas para consultar detalles de reportes. Superar un límite produce un mensaje para reducir el período o filtrar una cancha; no se entrega un archivo truncado.

Los indicadores de Reservas se agregan en SQL sobre el filtro completo y no sobre la página; sus detalles se completan en bloque para esa página.

## 11. Verificación y preparación de datos

### Pruebas disponibles

- `Reserva.Domain.Tests/Reporte/`: fórmulas, fechas, medias horas/medianoche, permisos, históricos, correcciones, agrupación por identidad y traducción SQL Server/MySQL.
- `Reserva.Api.Tests/Reporte/`: contratos/autorización y exportación real, incluidas hojas de movimientos y ausencia de IDs de responsable.
- `Reserva.Domain.Tests/Reserva/`: agregado completo frente a página, alcance, anuladas y detalle de gestión.
- `Reserva.Domain.Tests/Pago/`: cabecera opcional, primer cobro, abonos, correcciones, devoluciones e idempotencia.
- En frontend: pruebas de filtros de Reservas, detalle de reserva del calendario, permisos de menú y guard de roles.

Desde `ReservaCanchas_API`:

```powershell
dotnet test "Reserva.sln" --configuration Release
```

Desde `ReservaCanchas_admin_Web`:

```powershell
npm run build:development
npm test -- --watch=false --browsers=ChromeHeadless --include="src/app/features/reserva/components/filtros-reserva/filtros-reserva.component.spec.ts" --include="src/app/features/calendario/components/modal-detalle-reserva/modal-detalle-reserva.component.spec.ts" --include="src/app/shared/layouts/dashboard-layout/core/utils/menu.utils.spec.ts" --include="src/app/core/auth/guards/role.guard.spec.ts"
```

### Casos funcionales de comprobación

1. Crear una reserva sin cobro: consultar historial y comprobar que no crea Pago.
2. Registrar el primer cobro y un abono posterior: comprobar acumulados, saldo, movimientos y responsable.
3. Reintentar el mismo `idOperacion`: comprobar que no duplica dinero; cambiar el importe con ese ID debe rechazarse.
4. Corregir como proveedor: conservar diferencia y motivo; como operador la acción debe rechazarse.
5. Registrar una devolución: reducir el neto, cerrar el pendiente de la reserva cancelada y separar el reembolso del cobro.
6. Consultar una operación del lunes para una reserva del viernes: comprobar la diferencia temporal entre económico y movimientos.
7. Consultar un histórico sin responsable: no inventar identidad ni fecha del cobro original.
8. Comprobar franjas con cero demanda, medias horas y horarios que cruzan medianoche.
9. Como operador, comprobar que solo aparecen canchas asignadas aunque sus reservas provengan de distintos usuarios.
10. Cambiar filtros sin consultar: Excel debe permanecer deshabilitado; los atajos de movimientos deben consultar directamente.
11. Exportar más de una página: comprobar todas las filas, hojas correctas, formatos y ausencia de columnas de IDs internos.
12. En Reservas, comprobar anuladas, indicadores del filtro completo y apertura del detalle desde el código.

### Esquema y despliegue

Los reportes no agregan tablas propias. Requieren que la instalación ya tenga el modelo de Reserva/Pago/MovimientoPago descrito aquí.

Para instalaciones nuevas, los esquemas base están en el directorio HUGO: [SQLReservaCanchasV3.sql](../../SQLReservaCanchasV3.sql) y [MySQLReservaCanchasV3.sql](../../MySQLReservaCanchasV3.sql).

No hay migraciones automáticas al iniciar la API. Los scripts incrementales antiguos de movimientos citados en documentación previa no están actualmente en `Docs/Sql`; una base existente debe comprobar su esquema real frente al modelo vigente antes del despliegue. La consulta o exportación de un reporte no migra ni reconstruye datos históricos.
