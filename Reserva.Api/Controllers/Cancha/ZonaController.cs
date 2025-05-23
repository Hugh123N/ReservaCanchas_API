using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/Zona")]
    public class ZonaController : IZonaApplication
    {
        private readonly IZonaApplication _ZonaApplication;

        public ZonaController(IZonaApplication ZonaApplication)
            => _ZonaApplication = ZonaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetZonaDto>> Create(CreateZonaDto createDto)
            => await _ZonaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetZonaDto>> Update(UpdateZonaDto updateDto)
            => await _ZonaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ZonaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetZonaDto>> Get(int id)
            => await _ZonaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListZonaDto>>> List(int id)
            => await _ZonaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchZonaDto>>> Search(SearchParamsDto<SearchZonaFilterDto> searchParams)
            => await _ZonaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboZonaDto>>> SelectCombo()
            => await _ZonaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectZonaDto>>> Select(SearchParamsDto<SelectZonaFilterDto> searchParams)
            => await _ZonaApplication.Select(searchParams);

    }
}
