using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface ICanchaApplication
    {
        Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto);
        Task<ResponseDto<GetCanchaDto>> Update(UpdateCanchaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetCanchaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListCanchaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectCanchaDto>>> Select(SearchParamsDto<SelectCanchaFilterDto> searchParams);
        Task<ResponseDto<GetCanchaConfigDto>> GetConfig(int id);

    }
}

