using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IUsuarioApplication
    {
        Task<ResponseDto<GetUsuarioDto>> Create(CreateUsuarioDto createDto);
        Task<ResponseDto<GetUsuarioDto>> Update(UpdateUsuarioDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetUsuarioDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListUsuarioDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchUsuarioDto>>> Search(SearchParamsDto<SearchUsuarioFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboUsuarioDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectUsuarioDto>>> Select(SearchParamsDto<SelectUsuarioFilterDto> searchParams);

    }
}

