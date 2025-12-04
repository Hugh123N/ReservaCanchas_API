using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/ConfiguracionProveedor")]
    public class ConfiguracionProveedorController : IConfiguracionProveedorApplication
    {
        private readonly IConfiguracionProveedorApplication _ConfiguracionProveedorApplication;

        public ConfiguracionProveedorController(IConfiguracionProveedorApplication ConfiguracionProveedorApplication)
            => _ConfiguracionProveedorApplication = ConfiguracionProveedorApplication;

        [HttpPost]
        public async Task<ResponseDto<GetConfiguracionProveedorDto>> Create(CreateConfiguracionProveedorDto createDto)
            => await _ConfiguracionProveedorApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetConfiguracionProveedorDto>> Update(UpdateConfiguracionProveedorDto updateDto)
            => await _ConfiguracionProveedorApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ConfiguracionProveedorApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetConfiguracionProveedorDto>> Get(int id)
            => await _ConfiguracionProveedorApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListConfiguracionProveedorDto>>> List(int id)
            => await _ConfiguracionProveedorApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchConfiguracionProveedorDto>>> Search(SearchParamsDto<SearchConfiguracionProveedorFilterDto> searchParams)
            => await _ConfiguracionProveedorApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboConfiguracionProveedorDto>>> SelectCombo()
            => await _ConfiguracionProveedorApplication.SelectCombo();

    }
}
