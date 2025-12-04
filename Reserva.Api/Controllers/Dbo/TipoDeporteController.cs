using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/TipoDeporte")]
    public class TipoDeporteController : ITipoDeporteApplication
    {
        private readonly ITipoDeporteApplication _TipoDeporteApplication;

        public TipoDeporteController(ITipoDeporteApplication TipoDeporteApplication)
            => _TipoDeporteApplication = TipoDeporteApplication;

        [HttpPost]
        public async Task<ResponseDto<GetTipoDeporteDto>> Create(CreateTipoDeporteDto createDto)
            => await _TipoDeporteApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetTipoDeporteDto>> Update(UpdateTipoDeporteDto updateDto)
            => await _TipoDeporteApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _TipoDeporteApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetTipoDeporteDto>> Get(int id)
            => await _TipoDeporteApplication.Get(id);
        
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboTipoDeporteDto>>> SelectCombo()
            => await _TipoDeporteApplication.SelectCombo();
        
    }
}
