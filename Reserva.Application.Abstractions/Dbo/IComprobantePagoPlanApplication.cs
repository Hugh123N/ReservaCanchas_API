using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IComprobantePagoPlanApplication
    {
        Task<ResponseDto<GetComprobantePagoPlanDto>> Create(CreateComprobantePagoPlanDto createDto);
        Task<ResponseDto<GetComprobantePagoPlanDto>> Update(UpdateComprobantePagoPlanDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetComprobantePagoPlanDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListComprobantePagoPlanDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchComprobantePagoPlanDto>>> Search(SearchParamsDto<SearchComprobantePagoPlanFilterDto> searchParams);

    }
}

