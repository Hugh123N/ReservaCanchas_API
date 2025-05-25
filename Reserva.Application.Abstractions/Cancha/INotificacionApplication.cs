using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface INotificacionApplication
    {
        Task<ResponseDto<GetNotificacionDto>> Create(CreateNotificacionDto createDto);
        Task<ResponseDto<GetNotificacionDto>> Update(UpdateNotificacionDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetNotificacionDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListNotificacionDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchNotificacionDto>>> Search(SearchParamsDto<SearchNotificacionFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboNotificacionDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectNotificacionDto>>> Select(SearchParamsDto<SelectNotificacionFilterDto> searchParams);

    }
}

