using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IComisionApplication
    {
        Task<ResponseDto<GetComisionDto>> Create(CreateComisionDto createDto);
        Task<ResponseDto<GetComisionDto>> Update(UpdateComisionDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetComisionDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListComisionDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchComisionDto>>> Search(SearchParamsDto<SearchComisionFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboComisionDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectComisionDto>>> Select(SearchParamsDto<SelectComisionFilterDto> searchParams);

    }
}

