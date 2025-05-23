using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/Notificacion")]
    public class NotificacionController : INotificacionApplication
    {
        private readonly INotificacionApplication _NotificacionApplication;

        public NotificacionController(INotificacionApplication NotificacionApplication)
            => _NotificacionApplication = NotificacionApplication;

        [HttpPost]
        public async Task<ResponseDto<GetNotificacionDto>> Create(CreateNotificacionDto createDto)
            => await _NotificacionApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetNotificacionDto>> Update(UpdateNotificacionDto updateDto)
            => await _NotificacionApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _NotificacionApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetNotificacionDto>> Get(int id)
            => await _NotificacionApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListNotificacionDto>>> List(int id)
            => await _NotificacionApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchNotificacionDto>>> Search(SearchParamsDto<SearchNotificacionFilterDto> searchParams)
            => await _NotificacionApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboNotificacionDto>>> SelectCombo()
            => await _NotificacionApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectNotificacionDto>>> Select(SearchParamsDto<SelectNotificacionFilterDto> searchParams)
            => await _NotificacionApplication.Select(searchParams);

    }
}
