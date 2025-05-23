using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/Departamento")]
    public class DepartamentoController : IDepartamentoApplication
    {
        private readonly IDepartamentoApplication _DepartamentoApplication;

        public DepartamentoController(IDepartamentoApplication DepartamentoApplication)
            => _DepartamentoApplication = DepartamentoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetDepartamentoDto>> Create(CreateDepartamentoDto createDto)
            => await _DepartamentoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetDepartamentoDto>> Update(UpdateDepartamentoDto updateDto)
            => await _DepartamentoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _DepartamentoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetDepartamentoDto>> Get(int id)
            => await _DepartamentoApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListDepartamentoDto>>> List(int id)
            => await _DepartamentoApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchDepartamentoDto>>> Search(SearchParamsDto<SearchDepartamentoFilterDto> searchParams)
            => await _DepartamentoApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboDepartamentoDto>>> SelectCombo()
            => await _DepartamentoApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectDepartamentoDto>>> Select(SearchParamsDto<SelectDepartamentoFilterDto> searchParams)
            => await _DepartamentoApplication.Select(searchParams);

    }
}
