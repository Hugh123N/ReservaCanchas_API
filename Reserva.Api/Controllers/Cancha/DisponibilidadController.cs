using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Disponibilidad")]
    public class DisponibilidadController : IDisponibilidadApplication
    {
        private readonly IDisponibilidadApplication _DisponibilidadApplication;

        public DisponibilidadController(IDisponibilidadApplication DisponibilidadApplication)
            => _DisponibilidadApplication = DisponibilidadApplication;

        [HttpPost]
        public async Task<ResponseDto<GetDisponibilidadDto>> Create(CreateDisponibilidadDto createDto)
            => await _DisponibilidadApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetDisponibilidadDto>> Update(UpdateDisponibilidadDto updateDto)
            => await _DisponibilidadApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _DisponibilidadApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetDisponibilidadDto>> Get(int id)
            => await _DisponibilidadApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListDisponibilidadDto>>> List(int id)
            => await _DisponibilidadApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchDisponibilidadDto>>> Search(SearchParamsDto<SearchDisponibilidadFilterDto> searchParams)
            => await _DisponibilidadApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboDisponibilidadDto>>> SelectCombo()
            => await _DisponibilidadApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectDisponibilidadDto>>> Select(SearchParamsDto<SelectDisponibilidadFilterDto> searchParams)
            => await _DisponibilidadApplication.Select(searchParams);

    }
}
