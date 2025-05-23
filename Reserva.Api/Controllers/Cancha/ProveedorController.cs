using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/Proveedor")]
    public class ProveedorController : IProveedorApplication
    {
        private readonly IProveedorApplication _ProveedorApplication;

        public ProveedorController(IProveedorApplication ProveedorApplication)
            => _ProveedorApplication = ProveedorApplication;

        [HttpPost]
        public async Task<ResponseDto<GetProveedorDto>> Create(CreateProveedorDto createDto)
            => await _ProveedorApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetProveedorDto>> Update(UpdateProveedorDto updateDto)
            => await _ProveedorApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ProveedorApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetProveedorDto>> Get(int id)
            => await _ProveedorApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListProveedorDto>>> List(int id)
            => await _ProveedorApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchProveedorDto>>> Search(SearchParamsDto<SearchProveedorFilterDto> searchParams)
            => await _ProveedorApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboProveedorDto>>> SelectCombo()
            => await _ProveedorApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectProveedorDto>>> Select(SearchParamsDto<SelectProveedorFilterDto> searchParams)
            => await _ProveedorApplication.Select(searchParams);

    }
}
