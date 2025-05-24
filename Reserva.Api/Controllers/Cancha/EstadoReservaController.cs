using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/EstadoReserva")]
    public class EstadoReservaController : IEstadoReservaApplication
    {
        private readonly IEstadoReservaApplication _EstadoReservaApplication;

        public EstadoReservaController(IEstadoReservaApplication EstadoReservaApplication)
            => _EstadoReservaApplication = EstadoReservaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetEstadoReservaDto>> Create(CreateEstadoReservaDto createDto)
            => await _EstadoReservaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetEstadoReservaDto>> Update(UpdateEstadoReservaDto updateDto)
            => await _EstadoReservaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _EstadoReservaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetEstadoReservaDto>> Get(int id)
            => await _EstadoReservaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListEstadoReservaDto>>> List(int id)
            => await _EstadoReservaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchEstadoReservaDto>>> Search(SearchParamsDto<SearchEstadoReservaFilterDto> searchParams)
            => await _EstadoReservaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoReservaDto>>> SelectCombo()
            => await _EstadoReservaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectEstadoReservaDto>>> Select(SearchParamsDto<SelectEstadoReservaFilterDto> searchParams)
            => await _EstadoReservaApplication.Select(searchParams);

    }
}
