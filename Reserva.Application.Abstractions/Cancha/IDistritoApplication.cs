using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IDistritoApplication
    {
        Task<ResponseDto<GetDistritoDto>> Create(CreateDistritoDto createDto);
        Task<ResponseDto<GetDistritoDto>> Update(UpdateDistritoDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetDistritoDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListDistritoDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchDistritoDto>>> Search(SearchParamsDto<SearchDistritoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboDistritoDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectDistritoDto>>> Select(SearchParamsDto<SelectDistritoFilterDto> searchParams);

    }
}

