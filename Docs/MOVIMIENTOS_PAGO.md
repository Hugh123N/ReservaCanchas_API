# Pago como cabecera y MovimientoPago como detalle

## Preparación

- Instalación nueva: usar `../MySQLReservaCanchasV3.sql` del directorio HUGO.
- Base MySQL existente: ejecutar `Docs/Sql/20261113_detalle_movimientos_pago.mysql.sql`
  antes de desplegar esta versión. El archivo es un script de actualización, no se
  ejecuta automáticamente al iniciar la API.
- El script conserva los cuatro campos retirados de Pago en `PagoLegado20261113`
  para consulta histórica. Esa tabla es un archivo técnico, no una entidad operativa.
- La migración conserva las cabeceras anteriores, incluidos pagos con acumulado cero;
  la regla de no crear cabecera se aplica a las nuevas reservas sin cobros.

## Modelo

`Pago` conserva total, acumulado cobrado (`MontoAdelanto`), pendiente, acumulado devuelto,
estado, moneda, revisión y auditoría. Hay como máximo una cabecera por reserva.

`MovimientoPago` conserva el importe individual, tipo, acumulado anterior/resultante,
método y referencia, `IdUsuarioResponsable`, `RolResponsable` y auditoría. El backend
obtiene al responsable del JWT. La fecha de operación es `CreateDate`, en UTC. No se
agregan `IdUsuarioRegistro`, `FechaOperacion` ni `IdMovimientoOrigen`.

`IdUsuarioResponsable` y `RolResponsable` admiten nulos exclusivamente para información
histórica no identificable. Los movimientos nuevos se crean con un usuario autenticado.
No se intenta identificar personas por coincidencia de nombres.

## Flujos

1. Reserva normal sin dinero recibido: pendiente, conserva su precio en `Reserva.MontoTotal`,
   sin Pago ni movimientos.
2. Reserva inmediata: crea Pago y un COBRO con usuario, método y referencia.
3. Primer cobro posterior: crea Pago y su primer COBRO dentro de la transacción del command.
4. Nuevos abonos: agregan COBRO y actualizan acumulados, estado y revisión.
5. Edición de observaciones/horarios: conserva el dinero y recalcula el total/saldo con
   precios del servidor. No genera un cobro ficticio ni crea Pago si aún no existe.
6. Corrección: solo proveedor, conserva el ajuste firmado y su motivo. Modifica el
   acumulado, no representa cobro ni devolución. No puede bajar de lo ya reembolsado.
   Al no existir vínculo al movimiento original, sigue siendo un ajuste global; los
   futuros reportes deberán mostrarlo separado de los cobros por responsable/método.
7. Cancelación con devolución inmediata: valida política, revisión y dinero disponible;
   crea REEMBOLSO, incrementa `MontoReembolso`, cierra el pendiente y cancela la reserva.
   Este flujo registra dinero ya devuelto fuera de la plataforma; no ejecuta transferencias.

En COBRO/CORRECCION/SALDO_INICIAL, anterior/resultante corresponden a lo cobrado.
En REEMBOLSO corresponden a lo devuelto. El importe de cobros/devoluciones es positivo;
una corrección puede tener diferencia negativa.

## API

- `GET /api/Pago/reserva/{idReserva}/movimientos`: también funciona sin Pago; devuelve
  `idPago: 0`, precio, saldo, permisos e historial vacío, sin escribir datos.
- `GET /api/Pago/{idPago}/movimientos`: consulta por cabecera existente.
- `POST /api/Pago/registrar-cobro`: admite `idReserva` para el primer cobro o `idPago`
  para una cabecera existente. Recibe `idOperacion`, `revision`, `monto`,
  `codigoMetodoPago`, `referenciaOperacion` y `motivo`.
- `POST /api/Pago/corregir-registro`: requiere una cabecera existente.
- `POST /api/Reserva/liberar-reserva-operador`: incluye `revisionPago`, `idOperacion`,
  método y referencia de la devolución, además del importe y motivo.

Los identificadores de operación evitan duplicar cobros/devoluciones al reintentar.
La revisión de Pago y la auditoría concurrente de Reserva protegen sus actualizaciones.
Tras confirmar un primer cobro, consultar por reserva para obtener el ID definitivo
de Pago asignado durante el commit.

## Históricos y alcance

Los acumulados sin movimientos se incorporan como SALDO_INICIAL, sin inventar el
responsable, método o fecha original. La fecha del saldo inicial es su incorporación
al historial. Los reembolsos históricos permanecen en el resumen; no se inventan
desgloses de sus responsables/métodos.

El catálogo usa 01 Pendiente, 02 Confirmado, 03 Cancelado y 04 Expirado. Los estados
históricos 05 No presentado y 06 Completado se conservan. La migración reasigna códigos
por el nombre del estado, conservando IDs y sus reservas relacionadas.

Los reportes nuevos por responsable/método y la futura tabla PreReserva quedan para
otra implementación. El flujo actual de devolución es inmediato al cancelar; no se
incorpora una programación de reembolsos posteriores.
