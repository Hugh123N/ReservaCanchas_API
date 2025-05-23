using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.DetallePago;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/DetallePago")]
    public class DetallePagoController : IDetallePagoApplication
    {
        private readonly IDetallePagoApplication _DetallePagoApplication;

        public DetallePagoController(IDetallePagoApplication DetallePagoApplication)
            => _DetallePagoApplication = DetallePagoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetDetallePagoDto>> Create(CreateDetallePagoDto createDto)
            => await _DetallePagoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetDetallePagoDto>> Update(UpdateDetallePagoDto updateDto)
            => await _DetallePagoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _DetallePagoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetDetallePagoDto>> Get(int id)
            => await _DetallePagoApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListDetallePagoDto>>> List(int id)
            => await _DetallePagoApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchDetallePagoDto>>> Search(SearchParamsDto<SearchDetallePagoFilterDto> searchParams)
            => await _DetallePagoApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboDetallePagoDto>>> SelectCombo()
            => await _DetallePagoApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectDetallePagoDto>>> Select(SearchParamsDto<SelectDetallePagoFilterDto> searchParams)
            => await _DetallePagoApplication.Select(searchParams);

    }
}
