using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/TipoSuperficie")]
    public class TipoSuperficieController : ITipoSuperficieApplication
    {
        private readonly ITipoSuperficieApplication _TipoSuperficieApplication;

        public TipoSuperficieController(ITipoSuperficieApplication TipoSuperficieApplication)
            => _TipoSuperficieApplication = TipoSuperficieApplication;

        [HttpPost]
        public async Task<ResponseDto<GetTipoSuperficieDto>> Create(CreateTipoSuperficieDto createDto)
            => await _TipoSuperficieApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetTipoSuperficieDto>> Update(UpdateTipoSuperficieDto updateDto)
            => await _TipoSuperficieApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _TipoSuperficieApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetTipoSuperficieDto>> Get(int id)
            => await _TipoSuperficieApplication.Get(id);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboTipoSuperficieDto>>> SelectCombo()
            => await _TipoSuperficieApplication.SelectCombo();
        
    }
}
