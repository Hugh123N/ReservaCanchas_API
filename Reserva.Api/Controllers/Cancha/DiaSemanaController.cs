using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/DiaSemana")]
    public class DiaSemanaController : IDiaSemanaApplication
    {
        private readonly IDiaSemanaApplication _DiaSemanaApplication;

        public DiaSemanaController(IDiaSemanaApplication DiaSemanaApplication)
            => _DiaSemanaApplication = DiaSemanaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetDiaSemanaDto>> Create(CreateDiaSemanaDto createDto)
            => await _DiaSemanaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetDiaSemanaDto>> Update(UpdateDiaSemanaDto updateDto)
            => await _DiaSemanaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _DiaSemanaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetDiaSemanaDto>> Get(int id)
            => await _DiaSemanaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListDiaSemanaDto>>> List(int id)
            => await _DiaSemanaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchDiaSemanaDto>>> Search(SearchParamsDto<SearchDiaSemanaFilterDto> searchParams)
            => await _DiaSemanaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>> SelectCombo()
            => await _DiaSemanaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectDiaSemanaDto>>> Select(SearchParamsDto<SelectDiaSemanaFilterDto> searchParams)
            => await _DiaSemanaApplication.Select(searchParams);

    }
}
