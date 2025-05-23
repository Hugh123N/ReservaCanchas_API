using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/Distrito")]
    public class DistritoController : IDistritoApplication
    {
        private readonly IDistritoApplication _DistritoApplication;

        public DistritoController(IDistritoApplication DistritoApplication)
            => _DistritoApplication = DistritoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetDistritoDto>> Create(CreateDistritoDto createDto)
            => await _DistritoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetDistritoDto>> Update(UpdateDistritoDto updateDto)
            => await _DistritoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _DistritoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetDistritoDto>> Get(int id)
            => await _DistritoApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListDistritoDto>>> List(int id)
            => await _DistritoApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchDistritoDto>>> Search(SearchParamsDto<SearchDistritoFilterDto> searchParams)
            => await _DistritoApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboDistritoDto>>> SelectCombo()
            => await _DistritoApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectDistritoDto>>> Select(SearchParamsDto<SelectDistritoFilterDto> searchParams)
            => await _DistritoApplication.Select(searchParams);

    }
}
