using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/EstadoCancha")]
    public class EstadoCanchaController : IEstadoCanchaApplication
    {
        private readonly IEstadoCanchaApplication _EstadoCanchaApplication;

        public EstadoCanchaController(IEstadoCanchaApplication EstadoCanchaApplication)
            => _EstadoCanchaApplication = EstadoCanchaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetEstadoCanchaDto>> Create(CreateEstadoCanchaDto createDto)
            => await _EstadoCanchaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetEstadoCanchaDto>> Update(UpdateEstadoCanchaDto updateDto)
            => await _EstadoCanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _EstadoCanchaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetEstadoCanchaDto>> Get(int id)
            => await _EstadoCanchaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListEstadoCanchaDto>>> List(int id)
            => await _EstadoCanchaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchEstadoCanchaDto>>> Search(SearchParamsDto<SearchEstadoCanchaFilterDto> searchParams)
            => await _EstadoCanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoCanchaDto>>> SelectCombo()
            => await _EstadoCanchaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectEstadoCanchaDto>>> Select(SearchParamsDto<SelectEstadoCanchaFilterDto> searchParams)
            => await _EstadoCanchaApplication.Select(searchParams);

    }
}
