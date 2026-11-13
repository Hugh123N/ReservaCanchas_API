# Cobros y correcciones de reservas

> Actualización del modelo: ver [MOVIMIENTOS_PAGO.md](MOVIMIENTOS_PAGO.md) para la
> cabecera opcional, los responsables por movimiento, reembolsos y migración vigente.

## Preparación para probar

1. Detener la API que se ejecuta desde Visual Studio.
2. Ejecutar sobre la base de **ReservaCanchas** el script correspondiente al proveedor:
   - SQL Server: `Docs/Sql/20261112_movimientos_pago.sql`.
   - MySQL 8: `Docs/Sql/20261112_movimientos_pago.mysql.sql`.
3. Compilar y arrancar la API actualizada.
4. Arrancar el frontend de gestión actualizado y abrir una reserva desde Calendario.

Los scripts agregan `Pago.revision` y la tabla `MovimientoPago`; no reemplazan los
importes existentes. Deben aplicarse antes de iniciar la API actualizada. No hay
migraciones automáticas al arrancar.

## Funcionamiento

- **Editar reserva:** permite observaciones y horarios autorizados. No modifica lo
  cobrado. El precio se obtiene en el servidor cuando cambian los horarios y el saldo
  se recalcula conservando el acumulado recibido.
  Cancelar la edición restaura los datos ya cargados en memoria. Guardar utiliza el
  detalle completo devuelto por `PUT /api/Reserva` y mantiene abierto el modal en modo
  detalle, sin otro GET de reserva, disponibilidad o historial. El calendario se
  sincroniza al cerrar el modal si hubo cambios. Las validaciones de permisos,
  disponibilidad y versión del pago se siguen ejecutando en el servidor.
- **Registrar cobro:** el operador/proveedor ingresa dinero recibido **ahora**, con su
  método (efectivo, transferencia, Yape o Plin presencial) y una referencia opcional.
  No se ejecuta un cargo en una pasarela; se registra el dinero que se recibió fuera
  de la plataforma.
- **Corregir registro:** solo el proveedor del negocio. Ingresa el acumulado correcto
  y un motivo de al menos cinco caracteres. Conserva el anterior, el nuevo, la
  diferencia y quién realizó la corrección. No representa cobro ni devolución.
- **Historial:** muestra cobros iniciales creados desde calendario y todas las nuevas
  operaciones. Para pagos anteriores sin historial se agrega un movimiento
  `SALDO_INICIAL` al efectuar la primera operación, con el acumulado previo. Su fecha
  corresponde a la incorporación al historial, no a la fecha del cobro original.

El operador puede consultar las reservas de sus canchas asignadas y cobrar en las
de hoy/futuras. El proveedor puede cobrar/corregir también en reservas anteriores
que sigan vigentes. Las reservas canceladas/expiradas o pagos cancelados no admiten
cobros ni correcciones. Corregir no cancela ni desconfirma automáticamente una
reserva; el estado del pago sí se recalcula.

El mínimo de adelanto se valida al crear una reserva inmediata. Un abono posterior
no vuelve a exigir ese porcentaje para cada importe individual. Una corrección debe
reflejar el dinero real aunque el acumulado resulte inferior al mínimo. Una reserva
pendiente se confirma por cobro cuando el acumulado alcanza el mínimo configurado.

## API

| Método | Ruta | Permiso |
|---|---|---|
| GET | `/api/Pago/{id}/movimientos` | Proveedor / operador autorizado |
| POST | `/api/Pago/registrar-cobro` | Proveedor / operador autorizado |
| POST | `/api/Pago/corregir-registro` | Proveedor del negocio |

Los POST reciben:

```json
{
  "idPago": 5,
  "idOperacion": "a9dc2907-e053-45c9-b679-28c552c9bc72",
  "revision": 0,
  "monto": 20,
  "codigoMetodoPago": "02",
  "motivo": null
}
```

- En un cobro, `monto` es el importe adicional y el método es obligatorio.
- En una corrección, `monto` es el acumulado correcto y `motivo` es obligatorio.
- El historial devuelve la revisión vigente y los permisos efectivos.
- Una operación confirmada se puede reintentar con la misma clave y los mismos datos
  sin duplicar el dinero. La misma clave no se acepta con otro importe/tipo/método.
- Si cambia la revisión, se debe actualizar el detalle antes de volver a operar.
- Tras un POST exitoso el frontend vuelve a consultar historial y reserva después
  del commit, para mostrar los identificadores y las fechas definitivos.

`PUT /api/Reserva` requiere `revisionPago` y rechaza cambios al acumulado cobrado.
Las rutas genéricas de crear/actualizar/eliminar Pago y las anteriores de
confirmar/completar pago no permiten modificar pagos de reservas. Los clientes que
las usen para reservas deben migrar a las nuevas operaciones. Confirmar una reserva
por el endpoint anterior solo acepta conservar el acumulado existente.

Si el nuevo precio por horarios es inferior a lo recibido, se rechaza la edición
para resolver la diferencia mediante el flujo de devolución correspondiente. Una
reserva con cobros no puede eliminarse mediante cancelación simple.

## Casos para prueba manual

1. Crear reserva inmediata de S/ 100 con S/ 50 recibidos. Ver cobro inicial en historial.
2. Editar observaciones: el acumulado debe seguir en S/ 50 y no aparecer como input.
3. Como operador, registrar S/ 20: acumulado S/ 70, saldo S/ 30.
4. Intentar cobrar S/ 31: debe rechazarse. Cobrar S/ 30: queda pagada.
5. Como proveedor, corregir un registro de S/ 60 a S/ 50 con motivo: conservar el
   registro original y mostrar la diferencia de S/ -10 como corrección.
6. Corregir S/ 40 a S/ 50: debe figurar como corrección, no como nuevo cobro.
7. Como operador, comprobar que no aparece Corregir registro y que la API lo rechaza.
8. Abrir la misma reserva en dos sesiones y cobrar: la sesión con versión anterior
   debe actualizar antes de operar; no se debe sobrescribir el cobro de la otra.
9. Reintentar el mismo POST con el mismo `idOperacion`: no se debe sumar dos veces.
10. Abrir una reserva anterior sin movimientos y registrar una operación: comprobar
    la incorporación del saldo previo sin inventar fechas de cobros históricos.

## Comprobaciones automatizadas

Las acciones de editar, eliminar, confirmar y liberar reservas, y los nuevos
endpoints de movimientos, utilizan `Security.Authorize` para autenticar explícitamente
con Bearer. La configuración JWT reconoce `Roles` como claim de roles. Con esto se
cargan `UserId` y `UserIdNegocio` antes de ejecutar los controles de acceso del dominio.
Después de actualizar estos atributos/configuración, reiniciar la API para probar.

```powershell
dotnet build Reserva.sln --configuration Release
dotnet test Reserva.sln --configuration Release
```

Desde `ReservaCanchas_admin_Web`:

```powershell
npm run build:development
npm test -- --watch=false --browsers=ChromeHeadless --include="src/app/features/calendario/components/modal-detalle-reserva/modal-detalle-reserva.component.spec.ts"
```
