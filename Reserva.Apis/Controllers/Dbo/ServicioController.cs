using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Apis.Controllers.Dbo
{
    [ApiController]
    [Route("api/Servicio")]
    public class ServicioController : IServicioApplication
    {
        private readonly IServicioApplication _ServicioApplication;

        public ServicioController(IServicioApplication ServicioApplication)
            => _ServicioApplication = ServicioApplication;

        [HttpPost]
        public async Task<ResponseDto<GetServicioDto>> Create(CreateServicioDto createDto)
            => await _ServicioApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetServicioDto>> Update(UpdateServicioDto updateDto)
            => await _ServicioApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ServicioApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetServicioDto>> Get(int id)
            => await _ServicioApplication.Get(id);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboServicioDto>>> SelectCombo()
            => await _ServicioApplication.SelectCombo();

    }
}
