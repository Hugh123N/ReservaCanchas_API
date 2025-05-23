using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IRolApplication
    {
        Task<ResponseDto<GetRolDto>> Create(CreateRolDto createDto);
        Task<ResponseDto<GetRolDto>> Update(UpdateRolDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetRolDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListRolDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchRolDto>>> Search(SearchParamsDto<SearchRolFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboRolDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectRolDto>>> Select(SearchParamsDto<SelectRolFilterDto> searchParams);

    }
}

