using Azure;
using Microsoft.AspNetCore.Mvc;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Rol;
using Reserva.Entity.Models;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/Rol")]
    public class RolController : IRolApplication
    {
        private readonly IRolApplication _RolApplication;

        public RolController (IRolApplication RolApplication)
            => _RolApplication = RolApplication;

        [HttpPost]
        public async Task<ResponseDto<GetRolDto>> Create(CreateRolDto createDto)
            => await _RolApplication.Create(createDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _RolApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetRolDto>> Get(int id)
            => await _RolApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListRolDto>>> List(int id)
            => await _RolApplication.List(id);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboRolDto>>> SelectCombo()
            => await _RolApplication.SelectCombo();
        [HttpPut]
        public async Task<ResponseDto<GetRolDto>> Update(UpdateRolDto updateDto)
            => await _RolApplication.Update(updateDto);
    }
}
