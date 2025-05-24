using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Rol")]
    public class RolController : IRolApplication
    {
        private readonly IRolApplication _RolApplication;

        public RolController(IRolApplication RolApplication)
            => _RolApplication = RolApplication;

        [HttpPost]
        public async Task<ResponseDto<GetRolDto>> Create(CreateRolDto createDto)
            => await _RolApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetRolDto>> Update(UpdateRolDto updateDto)
            => await _RolApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _RolApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetRolDto>> Get(int id)
            => await _RolApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListRolDto>>> List(int id)
            => await _RolApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchRolDto>>> Search(SearchParamsDto<SearchRolFilterDto> searchParams)
            => await _RolApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboRolDto>>> SelectCombo()
            => await _RolApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectRolDto>>> Select(SearchParamsDto<SelectRolFilterDto> searchParams)
            => await _RolApplication.Select(searchParams);

    }
}
