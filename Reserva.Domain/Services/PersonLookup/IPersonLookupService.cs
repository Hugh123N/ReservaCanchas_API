using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PersonLookup;

namespace Reserva.Domain.Services.PersonLookup;

public interface IPersonLookupService
{
    Task<ResponseDto<DniLookupDto>> GetDniAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<ResponseDto<RucLookupDto>> GetRucAsync(string documentNumber, CancellationToken cancellationToken = default);
}
