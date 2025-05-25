using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/TipoProveedor")]
    public class TipoProveedorController : ITipoProveedorApplication
    {
        private readonly ITipoProveedorApplication _TipoProveedorApplication;

        public TipoProveedorController(ITipoProveedorApplication TipoProveedorApplication)
            => _TipoProveedorApplication = TipoProveedorApplication;

        [HttpPost]
        public async Task<ResponseDto<GetTipoProveedorDto>> Create(CreateTipoProveedorDto createDto)
            => await _TipoProveedorApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetTipoProveedorDto>> Update(UpdateTipoProveedorDto updateDto)
            => await _TipoProveedorApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _TipoProveedorApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetTipoProveedorDto>> Get(int id)
            => await _TipoProveedorApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListTipoProveedorDto>>> List(int id)
            => await _TipoProveedorApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchTipoProveedorDto>>> Search(SearchParamsDto<SearchTipoProveedorFilterDto> searchParams)
            => await _TipoProveedorApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboTipoProveedorDto>>> SelectCombo()
            => await _TipoProveedorApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectTipoProveedorDto>>> Select(SearchParamsDto<SelectTipoProveedorFilterDto> searchParams)
            => await _TipoProveedorApplication.Select(searchParams);

    }
}
