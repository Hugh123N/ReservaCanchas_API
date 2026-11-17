# Control de movimientos y responsables

Reporte exclusivo del proveedor en `/admin/reportes/control-movimientos`.
Se basa en `MOVIMIENTOS_PAGO.md` y utiliza el generador Excel de `Reserva.Common`.

## Alcance actual

Situación de reservas se trasladó al listado de **Reservas**, compartido por proveedor y operador.
Se retiró por completo su pestaña, período, consulta, datos y hoja del Excel de este reporte.
Consultar movimientos no ejecuta la búsqueda ni el cálculo de indicadores de reservas.

## Filtros y datos

- Desde/hasta por fecha local de operación (`MovimientoPago.CreateDate`, persistido en UTC).
- Responsable por GUID, o históricos sin identificar.
- Cancha del negocio, método, tipo y código completo/parcial de reserva.
- Hoy por defecto; accesos a hoy, semana a la fecha y mes a la fecha.
- Los tres accesos rápidos aplican las fechas y consultan inmediatamente; no requieren pulsar Consultar.
- Catálogo con el proveedor, operadores actuales/inactivos y responsables históricos de sus canchas.
- Resumen por responsable y cancha, desglose por método y detalle de operaciones.
- Nombre de cancha en todas las tablas y en el Excel.
- Los responsables se muestran por nombre, sin columnas de IDs internos en pantalla ni en el Excel; la agrupación y los filtros siguen usando su GUID internamente.
- Tablas paginadas localmente a 20 filas; apartados secundarios desplegables.

Cobrado = suma de COBRO filtrados. Reembolsado = suma de REEMBOLSO filtrados.
Neto = cobros menos reembolsos; puede ser negativo al devolver dinero cobrado antes.
Las reservas distintas se cuentan una sola vez aunque tengan varios abonos.

Las correcciones se contabilizan aparte como ajustes firmados del registro, no como dinero
nuevo ni como prueba de quién cometió el error original. El filtro de método no se aplica
a esos ajustes globales. Al elegir solo CORRECCION, no se acepta un método de pago.

`SALDO_INICIAL` se muestra como incorporación histórica, no como cobro del período ni como
dinero atribuible a una persona. No se deducen responsables comparando nombres.

El neto registrado no representa utilidad, entrega de caja ni conciliación bancaria.

## API y Excel

Endpoints con Bearer y rol Proveedor:

- `GET /api/Reporte/control-movimientos/filtros`
- `POST /api/Reporte/control-movimientos`
- `POST /api/Reporte/control-movimientos/excel`

```json
{
  "movimientos": {
    "fechaDesde": "2026-11-13",
    "fechaHasta": "2026-11-13",
    "idCancha": null,
    "idUsuarioResponsable": null,
    "soloSinResponsable": false,
    "idMetodoPago": null,
    "tipo": null,
    "codigoReserva": null
  }
}
```

El alcance se obtiene del JWT. Filtrar una persona no amplía las canchas autorizadas.
Se permiten hasta 366 días y 50 000 movimientos; si se exceden se solicita reducir el filtro,
sin devolver resultados truncados.

Excel con cuatro hojas: **Resumen**, **Responsables**, **Metodos** y **Movimientos**.
La hoja de movimientos separa entradas, salidas, ajustes e históricos. Anterior/resultante
son acumulados cobrados excepto en REEMBOLSO, donde son acumulados devueltos.
La exportación vuelve a consultar con los filtros aplicados e indica su generación.

No agrega tablas ni realiza escrituras de pagos o reservas.

Ver `RESERVAS_GESTION_SEARCH.md` para el listado que ahora contiene la situación de reservas.
