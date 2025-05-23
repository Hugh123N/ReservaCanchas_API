using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/TipoCancha")]
    public class TipoCanchaController : ITipoCanchaApplication
    {
        private readonly ITipoCanchaApplication _TipoCanchaApplication;

        public TipoCanchaController(ITipoCanchaApplication TipoCanchaApplication)
            => _TipoCanchaApplication = TipoCanchaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetTipoCanchaDto>> Create(CreateTipoCanchaDto createDto)
            => await _TipoCanchaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetTipoCanchaDto>> Update(UpdateTipoCanchaDto updateDto)
            => await _TipoCanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _TipoCanchaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetTipoCanchaDto>> Get(int id)
            => await _TipoCanchaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListTipoCanchaDto>>> List(int id)
            => await _TipoCanchaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchTipoCanchaDto>>> Search(SearchParamsDto<SearchTipoCanchaFilterDto> searchParams)
            => await _TipoCanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboTipoCanchaDto>>> SelectCombo()
            => await _TipoCanchaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectTipoCanchaDto>>> Select(SearchParamsDto<SelectTipoCanchaFilterDto> searchParams)
            => await _TipoCanchaApplication.Select(searchParams);

    }
}
