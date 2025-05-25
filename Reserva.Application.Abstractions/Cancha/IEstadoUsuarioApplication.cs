using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IEstadoUsuarioApplication
    {
        Task<ResponseDto<GetEstadoUsuarioDto>> Create(CreateEstadoUsuarioDto createDto);
        Task<ResponseDto<GetEstadoUsuarioDto>> Update(UpdateEstadoUsuarioDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetEstadoUsuarioDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListEstadoUsuarioDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchEstadoUsuarioDto>>> Search(SearchParamsDto<SearchEstadoUsuarioFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboEstadoUsuarioDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectEstadoUsuarioDto>>> Select(SearchParamsDto<SelectEstadoUsuarioFilterDto> searchParams);

    }
}

