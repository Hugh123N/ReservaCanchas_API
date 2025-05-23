using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/MetodoPago")]
    public class MetodoPagoController : IMetodoPagoApplication
    {
        private readonly IMetodoPagoApplication _MetodoPagoApplication;

        public MetodoPagoController(IMetodoPagoApplication MetodoPagoApplication)
            => _MetodoPagoApplication = MetodoPagoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetMetodoPagoDto>> Create(CreateMetodoPagoDto createDto)
            => await _MetodoPagoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetMetodoPagoDto>> Update(UpdateMetodoPagoDto updateDto)
            => await _MetodoPagoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _MetodoPagoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetMetodoPagoDto>> Get(int id)
            => await _MetodoPagoApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListMetodoPagoDto>>> List(int id)
            => await _MetodoPagoApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchMetodoPagoDto>>> Search(SearchParamsDto<SearchMetodoPagoFilterDto> searchParams)
            => await _MetodoPagoApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboMetodoPagoDto>>> SelectCombo()
            => await _MetodoPagoApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectMetodoPagoDto>>> Select(SearchParamsDto<SelectMetodoPagoFilterDto> searchParams)
            => await _MetodoPagoApplication.Select(searchParams);

    }
}
