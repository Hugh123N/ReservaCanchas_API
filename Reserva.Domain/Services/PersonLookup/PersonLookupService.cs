using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PersonLookup;
using System.Text.Json;

namespace Reserva.Domain.Services.PersonLookup;

public sealed class PersonLookupService : IPersonLookupService
{
    private readonly IEnumerable<IPersonLookupStrategy> _strategies;
    private readonly IOptionsSnapshot<PersonLookupOptions> _options;
    private readonly ILogger<PersonLookupService> _logger;

    public PersonLookupService(
        IEnumerable<IPersonLookupStrategy> strategies,
        IOptionsSnapshot<PersonLookupOptions> options,
        ILogger<PersonLookupService> logger)
    {
        _strategies = strategies;
        _options = options;
        _logger = logger;
    }

    public Task<ResponseDto<DniLookupDto>> GetDniAsync(string documentNumber, CancellationToken cancellationToken = default)
        => LookupAsync(documentNumber, 8, "DNI", (strategy, token) => strategy.GetDniAsync(documentNumber, token), cancellationToken);

    public Task<ResponseDto<RucLookupDto>> GetRucAsync(string documentNumber, CancellationToken cancellationToken = default)
        => LookupAsync(documentNumber, 11, "RUC", (strategy, token) => strategy.GetRucAsync(documentNumber, token), cancellationToken);

    private async Task<ResponseDto<T>> LookupAsync<T>(
        string documentNumber,
        int length,
        string documentType,
        Func<IPersonLookupStrategy, CancellationToken, Task<T?>> lookup,
        CancellationToken cancellationToken) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = new ResponseDto<T>();
        if (string.IsNullOrEmpty(documentNumber) || documentNumber.Length != length ||
            documentNumber.Any(character => character < '0' || character > '9'))
        {
            response.AddErrorResult($"El {documentType} debe contener exactamente {length} dígitos numéricos.");
            return response;
        }

        try
        {
            var options = _options.Value;
            var strategy = _strategies.FirstOrDefault(candidate =>
                string.Equals(candidate.ProviderName, options.ActiveProvider, StringComparison.OrdinalIgnoreCase));

            if (strategy == null || !options.Providers.TryGetValue(strategy.ProviderName, out var provider) || !provider.Enabled)
                throw new PersonLookupException("No hay un proveedor de consulta activo y habilitado.");

            var data = await lookup(strategy, cancellationToken);
            if (data == null)
                response.AddErrorResult($"No se encontraron datos para el {documentType} indicado.");
            else
                response.UpdateData(data);
        }
        catch (PersonLookupException exception)
        {
            response.AddErrorResult(exception.Message);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            response.AddErrorResult("El proveedor de consulta excedió el tiempo de espera. Intenta nuevamente.");
        }
        catch (HttpRequestException)
        {
            _logger.LogWarning("No se pudo establecer comunicación con el proveedor de consulta de documentos.");
            response.AddErrorResult("No se pudo conectar con el proveedor de consulta. Intenta nuevamente.");
        }
        catch (JsonException)
        {
            _logger.LogWarning("El proveedor de consulta de documentos devolvió un JSON inválido.");
            response.AddErrorResult("El proveedor devolvió una respuesta de consulta inválida.");
        }
        catch (OptionsValidationException)
        {
            response.AddErrorResult("La configuración del servicio de consulta no es válida.");
        }

        return response;
    }
}
