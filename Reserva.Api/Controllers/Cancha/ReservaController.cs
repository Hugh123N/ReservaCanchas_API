using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Reserva")]
    public class ReservaController : IReservaApplication
    {
        private readonly IReservaApplication _ReservaApplication;

        public ReservaController(IReservaApplication ReservaApplication)
            => _ReservaApplication = ReservaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetReservaDto>> Create(CreateReservaDto createDto)
            => await _ReservaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto)
            => await _ReservaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ReservaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetReservaDto>> Get(int id)
            => await _ReservaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id)
            => await _ReservaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchReservaDto>>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams)
            => await _ReservaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> SelectCombo()
            => await _ReservaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectReservaDto>>> Select(SearchParamsDto<SelectReservaFilterDto> searchParams)
            => await _ReservaApplication.Select(searchParams);

    }
}
