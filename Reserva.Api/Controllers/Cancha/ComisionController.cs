using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Comision")]
    public class ComisionController : IComisionApplication
    {
        private readonly IComisionApplication _ComisionApplication;

        public ComisionController(IComisionApplication ComisionApplication)
            => _ComisionApplication = ComisionApplication;

        [HttpPost]
        public async Task<ResponseDto<GetComisionDto>> Create(CreateComisionDto createDto)
            => await _ComisionApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetComisionDto>> Update(UpdateComisionDto updateDto)
            => await _ComisionApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ComisionApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetComisionDto>> Get(int id)
            => await _ComisionApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListComisionDto>>> List(int id)
            => await _ComisionApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchComisionDto>>> Search(SearchParamsDto<SearchComisionFilterDto> searchParams)
            => await _ComisionApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboComisionDto>>> SelectCombo()
            => await _ComisionApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectComisionDto>>> Select(SearchParamsDto<SelectComisionFilterDto> searchParams)
            => await _ComisionApplication.Select(searchParams);

    }
}
