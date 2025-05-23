using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IEstadoReservaApplication
    {
        Task<ResponseDto<GetEstadoReservaDto>> Create(CreateEstadoReservaDto createDto);
        Task<ResponseDto<GetEstadoReservaDto>> Update(UpdateEstadoReservaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetEstadoReservaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListEstadoReservaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchEstadoReservaDto>>> Search(SearchParamsDto<SearchEstadoReservaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboEstadoReservaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectEstadoReservaDto>>> Select(SearchParamsDto<SelectEstadoReservaFilterDto> searchParams);

    }
}

