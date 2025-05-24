using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.EstadoPago;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/EstadoPago")]
    public class EstadoPagoController : IEstadoPagoApplication
    {
        private readonly IEstadoPagoApplication _EstadoPagoApplication;

        public EstadoPagoController(IEstadoPagoApplication EstadoPagoApplication)
            => _EstadoPagoApplication = EstadoPagoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetEstadoPagoDto>> Create(CreateEstadoPagoDto createDto)
            => await _EstadoPagoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetEstadoPagoDto>> Update(UpdateEstadoPagoDto updateDto)
            => await _EstadoPagoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _EstadoPagoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetEstadoPagoDto>> Get(int id)
            => await _EstadoPagoApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListEstadoPagoDto>>> List(int id)
            => await _EstadoPagoApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchEstadoPagoDto>>> Search(SearchParamsDto<SearchEstadoPagoFilterDto> searchParams)
            => await _EstadoPagoApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoPagoDto>>> SelectCombo()
            => await _EstadoPagoApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectEstadoPagoDto>>> Select(SearchParamsDto<SelectEstadoPagoFilterDto> searchParams)
            => await _EstadoPagoApplication.Select(searchParams);

    }
}
