using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Apis.Controllers.Dbo
{
    [ApiController]
    [Route("api/HorarioCancha")]
    public class HorarioCanchaController : IHorarioCanchaApplication
    {
        private readonly IHorarioCanchaApplication _HorarioCanchaApplication;

        public HorarioCanchaController(IHorarioCanchaApplication HorarioCanchaApplication)
            => _HorarioCanchaApplication = HorarioCanchaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetHorarioCanchaDto>> Create(CreateHorarioCanchaDto createDto)
            => await _HorarioCanchaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetHorarioCanchaDto>> Update(UpdateHorarioCanchaDto updateDto)
            => await _HorarioCanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _HorarioCanchaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetHorarioCanchaDto>> Get(int id)
            => await _HorarioCanchaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListHorarioCanchaDto>>> List(int id)
            => await _HorarioCanchaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchHorarioCanchaDto>>> Search(SearchParamsDto<SearchHorarioCanchaFilterDto> searchParams)
            => await _HorarioCanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboHorarioCanchaDto>>> SelectCombo()
            => await _HorarioCanchaApplication.SelectCombo();

    }
}
