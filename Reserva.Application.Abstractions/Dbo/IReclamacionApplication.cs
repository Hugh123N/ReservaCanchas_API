using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IReclamacionApplication
    {
        Task<ResponseDto<GetReclamacionDto>> Create(CreateReclamacionDto createDto, CancellationToken cancellationToken);
        Task<ResponseDto<GetReclamacionDto>> Get(int id, CancellationToken cancellationToken);
        Task<ResponseDto<SearchResultDto<SearchReclamacionDto>>> Search(SearchParamsDto<SearchReclamacionFilterDto> searchParams, CancellationToken cancellationToken);
        Task<ResponseDto> Atender(int id, AtenderReclamacionDto atenderDto, CancellationToken cancellationToken);
    }
}
