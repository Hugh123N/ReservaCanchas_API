using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Cancha")]
    public class CanchaController : ICanchaApplication
    {
        private readonly ICanchaApplication _CanchaApplication;

        public CanchaController(ICanchaApplication CanchaApplication)
            => _CanchaApplication = CanchaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto)
            => await _CanchaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetCanchaDto>> Update(UpdateCanchaDto updateDto)
            => await _CanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _CanchaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetCanchaDto>> Get(int id)
            => await _CanchaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListCanchaDto>>> List(int id)
            => await _CanchaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams)
            => await _CanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> SelectCombo()
            => await _CanchaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectCanchaDto>>> Select(SearchParamsDto<SelectCanchaFilterDto> searchParams)
            => await _CanchaApplication.Select(searchParams);

    }
}
