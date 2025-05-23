using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/Provincia")]
    public class ProvinciaController : IProvinciaApplication
    {
        private readonly IProvinciaApplication _ProvinciaApplication;

        public ProvinciaController(IProvinciaApplication ProvinciaApplication)
            => _ProvinciaApplication = ProvinciaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetProvinciaDto>> Create(CreateProvinciaDto createDto)
            => await _ProvinciaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetProvinciaDto>> Update(UpdateProvinciaDto updateDto)
            => await _ProvinciaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ProvinciaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetProvinciaDto>> Get(int id)
            => await _ProvinciaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListProvinciaDto>>> List(int id)
            => await _ProvinciaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchProvinciaDto>>> Search(SearchParamsDto<SearchProvinciaFilterDto> searchParams)
            => await _ProvinciaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboProvinciaDto>>> SelectCombo()
            => await _ProvinciaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectProvinciaDto>>> Select(SearchParamsDto<SelectProvinciaFilterDto> searchParams)
            => await _ProvinciaApplication.Select(searchParams);

    }
}
