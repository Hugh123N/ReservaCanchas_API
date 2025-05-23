using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/GananciaProveedor")]
    public class GananciaProveedorController : IGananciaProveedorApplication
    {
        private readonly IGananciaProveedorApplication _GananciaProveedorApplication;

        public GananciaProveedorController(IGananciaProveedorApplication GananciaProveedorApplication)
            => _GananciaProveedorApplication = GananciaProveedorApplication;

        [HttpPost]
        public async Task<ResponseDto<GetGananciaProveedorDto>> Create(CreateGananciaProveedorDto createDto)
            => await _GananciaProveedorApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetGananciaProveedorDto>> Update(UpdateGananciaProveedorDto updateDto)
            => await _GananciaProveedorApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _GananciaProveedorApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetGananciaProveedorDto>> Get(int id)
            => await _GananciaProveedorApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListGananciaProveedorDto>>> List(int id)
            => await _GananciaProveedorApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchGananciaProveedorDto>>> Search(SearchParamsDto<SearchGananciaProveedorFilterDto> searchParams)
            => await _GananciaProveedorApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboGananciaProveedorDto>>> SelectCombo()
            => await _GananciaProveedorApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectGananciaProveedorDto>>> Select(SearchParamsDto<SelectGananciaProveedorFilterDto> searchParams)
            => await _GananciaProveedorApplication.Select(searchParams);

    }
}
