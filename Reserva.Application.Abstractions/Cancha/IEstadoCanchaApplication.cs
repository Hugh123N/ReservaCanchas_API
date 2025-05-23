using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IEstadoCanchaApplication
    {
        Task<ResponseDto<GetEstadoCanchaDto>> Create(CreateEstadoCanchaDto createDto);
        Task<ResponseDto<GetEstadoCanchaDto>> Update(UpdateEstadoCanchaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetEstadoCanchaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListEstadoCanchaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchEstadoCanchaDto>>> Search(SearchParamsDto<SearchEstadoCanchaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboEstadoCanchaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectEstadoCanchaDto>>> Select(SearchParamsDto<SelectEstadoCanchaFilterDto> searchParams);

    }
}

