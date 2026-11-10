# Consulta de DNI y RUC

## Uso interno

Inyectar `IPersonLookupService` en el servicio o handler que necesite consultar un documento.
Sus métodos retornan el `ResponseDto<T>` habitual:

- `GetDniAsync(documentNumber, cancellationToken)`: exactamente 8 dígitos ASCII.
- `GetRucAsync(documentNumber, cancellationToken)`: exactamente 11 dígitos ASCII.

```csharp
var dniResponse = await _personLookupService.GetDniAsync(dni, cancellationToken);
var rucResponse = await _personLookupService.GetRucAsync(ruc, cancellationToken);
```

Los números son strings para conservar los ceros iniciales. La validación se centraliza en
`IPersonLookupService`. El registro de dependencias se realiza mediante `AddPersonLookup(configuration)`.

### Consumidor: calendario de gestión

La búsqueda existente del calendario recibe `GET /api/Calendario/buscar-cliente?dni={dni}`.
`BuscarClienteQueryHandler` busca primero un cliente activo con `TipoDoc = DNI` y el número exacto.
Si existe, retorna sus datos y su ID sin llamar al proveedor; si no, consulta internamente
`IPersonLookupService.GetDniAsync` y retorna un único `ClientDto` con `EsNuevoCliente = true` e ID null.
La consulta por sí sola no registra clientes.

El modal muestra nombres, apellidos y DNI como texto. El teléfono es editable y obligatorio para
clientes nuevos y existentes. Al confirmar la reserva se registra el cliente nuevo con su DNI o se
reutiliza el existente y se actualiza su teléfono dentro de la transacción de la reserva.

### DNI: `DniLookupDto`

```json
{
  "firstName": "NOMBRES",
  "lastName": "APELLIDO PATERNO",
  "maternalLastName": "APELLIDO MATERNO",
  "documentNumber": "00000001"
}
```

`maternalLastName` puede ser null cuando no lo devuelve el proveedor.

### RUC: `RucLookupDto`

```json
{
  "name": "RAZÓN SOCIAL",
  "documentNumber": "20000000001"
}
```

Los DTOs de respuesta contienen únicamente estos datos. No se almacenan las consultas en la base de datos.

## Configuración

La sección `PersonLookup` está en `Reserva.Api/appsettings.json`:

- `ActiveProvider`: `ApisPeruPortal` (valor inicial) o `Factiliza`.
- `TimeoutSeconds`: entre 1 y 60; valor inicial 15.
- `Providers:{nombre}:Enabled`: habilita o deshabilita cada proveedor.
- `Providers:{nombre}:BaseUrl`: URL base HTTPS del proveedor.
- `Providers:{nombre}:Token`: token real obtenido desde el portal del proveedor. Los valores iniciales están vacíos.

Para usar Factiliza, establecer `ActiveProvider` en `Factiliza`, habilitar su entrada y configurar su token.
Se puede deshabilitar APIsPERU mediante `Providers:ApisPeruPortal:Enabled = false`.
El servicio utiliza únicamente el proveedor seleccionado; no cambia automáticamente de proveedor ante errores.

Los tokens pueden configurarse en el appsettings del entorno o mediante las variables:

```text
PersonLookup__Providers__ApisPeruPortal__Token
PersonLookup__Providers__Factiliza__Token
```

APIsPERU envía el token como query string; Factiliza utiliza `Authorization: Bearer`.
El cliente HTTP de esta integración tiene deshabilitado el logging automático de URLs y las redirecciones.
Los errores devueltos por el servicio no incluyen el contenido crudo del proveedor, tokens ni excepciones internas.

## Arquitectura y extensión

```text
Servicio o handler consumidor → IPersonLookupService
    → IPersonLookupStrategy → ApisPeruPortalStrategy / FactilizaStrategy
```

Las estrategias están en `Reserva.Domain/Services/PersonLookup/Providers`.
Para integrar otro proveedor:

1. Implementar `IPersonLookupStrategy` con un `ProviderName` único y ambos métodos.
2. Registrar la implementación en `AddPersonLookup`, como las estrategias existentes.
3. Agregar su entrada en `PersonLookup:Providers` y seleccionarlo en `ActiveProvider`.

Los consumidores del servicio conservan la misma interfaz al cambiar de proveedor.

## Verificación

```sh
dotnet build Reserva.sln
dotnet test Reserva.Domain.Tests/Reserva.Domain.Tests.csproj
```

Las pruebas utilizan HTTP simulado para verificar ambos contratos y métodos de autenticación,
validación de documentos, selección de proveedor, errores HTTP/JSON, timeout y cancelación.
Para una consulta real se debe configurar el token del proveedor activo.

## Contratos externos consultados

- APIsPERU Portal: https://dniruc.apisperu.com/doc/swagger
- Factiliza DNI: https://docs.factiliza.com/api-consulta/endpoint/dni
- Factiliza RUC: https://docs.factiliza.com/api-consulta/endpoint/ruc
