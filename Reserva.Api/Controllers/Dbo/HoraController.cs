using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Hora")]
    public class HoraController
    {
        private readonly IHoraApplication _HoraApplication;

        public HoraController(IHoraApplication HoraApplication)
            => _HoraApplication = HoraApplication;

        [HttpGet("{id}")]
        public async Task<ResponseDto<GetHoraDto>> Get(int id)
            => await _HoraApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListHoraDto>>> List(int id)
            => await _HoraApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchHoraDto>>> Search(SearchParamsDto<SearchHoraFilterDto> searchParams)
            => await _HoraApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboHoraDto>>> SelectCombo()
            => await _HoraApplication.SelectCombo();
        
    }
}
