using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IPagoPlanApplication
    {
        Task<ResponseDto<GetPagoPlanDto>> Create(CreatePagoPlanDto createDto);
        Task<ResponseDto<GetPagoPlanDto>> Update(UpdatePagoPlanDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetPagoPlanDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListPagoPlanDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchPagoPlanDto>>> Search(SearchParamsDto<SearchPagoPlanFilterDto> searchParams);

    }
}

