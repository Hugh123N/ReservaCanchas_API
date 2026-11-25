# Libro de Reclamaciones de ReservaFast

## Instalación

1. Crear la tabla del motor configurado. Para SQL Server, la referencia es `Docs/Sql/RECLAMACION_SQLSERVER.sql`. Para MySQL, utilizar una tabla InnoDB con `IdReclamacion INT AUTO_INCREMENT` y `NumeroHoja VARCHAR(20) NULL` como columna normal, no generada. Si esa tabla ya existe, no es necesario recrearla para esta corrección.
2. Desplegar la API y el frontend de gestión. No se aplican migraciones ni cambios de datos automáticamente desde el código.
3. Configurar el SMTP existente (`EmailSettings`) para las copias y notificaciones.
4. El correo del comercio usa `LibroReclamaciones:EmailAtencion` o, si no se configura, `hughhnp@gmail.com`. En variables de entorno corresponde a `LibroReclamaciones__EmailAtencion`.

El script de SQL Server crea una tabla, sin modificar datos existentes, con comentarios junto a cada columna y separación por grupos. Puede ejecutarse nuevamente sin borrar una tabla ya creada. No actualiza estructuras anteriores si cambian en el futuro. Las sentencias `AUTO_INCREMENT`, `TINYINT(1)`, `ENGINE=InnoDB` y las colaciones `utf8mb4` son de MySQL, no de SQL Server.

## Rutas y contratos

| Acceso | Ruta | Finalidad |
|---|---|---|
| Público | `/libro-de-reclamaciones` | Formulario y constancia al registrar |
| Admin | `/admin/gestion/reclamaciones` | Búsqueda, hoja completa y registro de atención |
| Anónimo | `POST /api/Reclamacion` | Registra y devuelve la hoja completa y el resultado del envío de copia |
| Admin | `POST /api/Reclamacion/search` | `SearchParamsDto<SearchReclamacionFilterDto>` con `filter`, `page` y `sort` (máximo 100 registros por página) |
| Admin | `GET /api/Reclamacion/{id}` | Consulta privada de la hoja |
| Admin | `PUT /api/Reclamacion/{id}/atencion` | Registra atención y retorna `ResponseDto`, sin ejecutar una consulta adicional |

`Create` y `Get` retornan `ResponseDto<GetReclamacionDto>`. `Search` retorna `ResponseDto<SearchResultDto<SearchReclamacionDto>>`. `Atender` retorna únicamente `ResponseDto` (mensajes e indicador de validez, sin `data`). No existe consulta pública por ID, edición del relato original ni endpoint de eliminación.

La búsqueda sigue el mismo contrato de `Operador`:

```json
{
  "filter": { "texto": "", "estado": "PENDIENTE" },
  "page": { "page": 1, "pageSize": 20 },
  "sort": [{ "property": "fechaRegistro", "direction": "desc" }]
}
```

Los datos de búsqueda se devuelven como `items`, `total`, `page` y `pageSize`. El frontend administrativo consume este contrato y, después de una atención válida, recarga la hoja mediante `Get` y el listado con los filtros aplicados. Si el cambio de estado deja vacía la última página filtrada, consulta la última página disponible. Los modelos y las solicitudes de atención no incluyen `version`.

### Datos del consumidor y de la solicitud

- `nombres`, `tipDoc`, `nroDoc`, `domicilio`, `email`.
- `tipDoc`: `1` DNI (8 dígitos), `4` carné de extranjería, `7` pasaporte. Los documentos se conservan como texto.
- `telefono` opcional; `esMenor` y `representante` (obligatorio si es menor).
- `descripcionServicio`, `montoReclamado` opcional en PEN, `referenciaOperacion` opcional.
- `tipo`: `RECLAMO` o `QUEJA`; `detalle` y `pedido` separados.

La API toma los datos del comercio de su configuración/código y los conserva en cada hoja; el consumidor no puede enviarlos para sustituirlos. El formulario publica los valores iniciales en `src/app/features/reclamaciones/core/reclamacion.constants.ts`; si cambian los datos del comercio, actualizar también esa presentación. Las hojas anteriores conservan su instantánea.

### Número y fechas

`IdReclamacion` es el correlativo autogenerado por el motor. `NumeroHoja` es una columna normal nullable de hasta 20 caracteres, no una columna calculada. EF la mapea con `ValueGeneratedNever()`.

El repositorio inserta la hoja con `NumeroHoja = NULL` y obtiene el ID. El handler asigna `RF-{IdReclamacion}-{año peruano}` y registra la actualización; el Unit of Work guarda y confirma todo en la misma transacción, antes de notificar por correo. No se utiliza `MAX + 1`, no se reinicia el correlativo por año y puede contener saltos por transacciones revertidas.

El índice único conserva la unicidad de los números definitivos. En SQL Server debe ser filtrado por `NumeroHoja IS NOT NULL`, para permitir los valores transitorios nulos, como ya permite MySQL.

Si existe una instalación antigua de SQL Server con `NumeroHoja` calculada, hay que migrar esa columna a `varchar(20) NULL` conservando los números existentes antes de usar esta versión; el script de referencia no altera tablas existentes.

`FechaRegistro` se asigna en la API. En SQL Server se usa `datetimeoffset` y en MySQL `datetime(6)`; los instantes se presentan en horario de Perú (UTC-05:00). La fecha de comunicación no puede ser anterior al registro ni futura.

## Atención y respuesta manual

Estados: `PENDIENTE` → `EN_ATENCION` → `RESPONDIDA`. También puede registrarse directamente una respuesta a una hoja pendiente.

El administrador envía la respuesta por su canal de correo y registra:

- Estado y observaciones/acciones del proveedor.
- Texto de la respuesta enviada.
- Fecha real de comunicación y referencia para ubicar el correo/carta o su constancia.

Guardar atención no envía una respuesta ni ejecuta devoluciones. El administrador conserva el correo enviado y su evidencia. Una respuesta cerrada no puede sobrescribirse. Las observaciones son parte de la sección del proveedor, no notas internas secretas.

## Copias, impresión y permisos

- La API confirma la transacción antes de intentar enviar correos. La Application coordina el comando y la notificación; la construcción y el envío de la copia están en `NotificacionService.Reclamaciones.cs`, dentro del servicio de notificaciones existente. Los fallos de SMTP no revierten la hoja.
- El consumidor recibe una copia HTML con la información completa; el comercio recibe una notificación independiente, sin compartir destinatarios entre ambos correos.
- `copiaEnviada` indica si el envío SMTP fue aceptado. Si falla, la pantalla mantiene la constancia imprimible y advierte que se debe conservar.
- El botón de impresión clona el DOM escapado por Angular en una ventana de impresión. Permite imprimir o guardar PDF desde el navegador, sin bibliotecas adicionales.
- Las consultas y la atención requieren rol Admin tanto en el controller como en el dominio. No basta con conocer el correlativo.
- El formulario no requiere cuenta, plan ni aceptación comercial. El registro se envía sin token; una sesión vencida no debe redirigir esta página al login.
- Los endpoints de texto libre están marcados con `PlainTextPayload`: mantienen límite de tamaño, validación de datos, rate limit y SQL parametrizado de EF, pero no rechazan puntuación o relatos por heurísticas SQL. Todos los datos se presentan como texto escapado, incluido el correo y la impresión.

## Base del formato

Secciones de identificación del consumidor, identificación del servicio, detalle/pedido y acciones/respuesta del proveedor, conforme al formato de referencia publicado por Indecopi:

- https://consumidor.gob.pe/libro-de-reclamaciones/
- https://www.gob.pe/institucion/pcm/normas-legales/3346742-101-2022-pcm

La página y las constancias indican 15 días hábiles improrrogables para responder. No calculan una fecha límite automática, para no omitir feriados. La confirmación de recepción no sustituye la respuesta al reclamo.

## Verificación de despliegue

Comprobar en el entorno desplegado que se puede registrar sin sesión, guardar/imprimir la copia, recibir los correos y consultar la hoja como Admin. La compilación no verifica la ejecución del script ni la conectividad de SQL o SMTP. No utilizar credenciales de proveedor para revisar la atención administrativa del libro.
