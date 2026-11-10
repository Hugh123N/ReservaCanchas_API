using Reserva.Dto.Dbo.PersonLookup;

namespace Reserva.Domain.Services.PersonLookup;

public interface IPersonLookupStrategy
{
    string ProviderName { get; }
    Task<DniLookupDto?> GetDniAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<RucLookupDto?> GetRucAsync(string documentNumber, CancellationToken cancellationToken = default);
}
